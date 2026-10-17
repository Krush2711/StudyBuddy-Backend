using Microsoft.AspNetCore.Authentication;
using StudyBuddy.API.Authentication;
using StudyBuddy.API.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using StudyBuddy.API.Middleware;
using StudyBuddy.API.Seeding;
using StudyBuddy.BLL.Interfaces;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.BLL.Services;
using StudyBuddy.BLL.Services.Add;
using StudyBuddy.BLL.Services.Auth;
using StudyBuddy.DAL.Context;
using StudyBuddy.DAL.Interfaces;
using StudyBuddy.DAL.Interfaces.Auth;
using StudyBuddy.DAL.Repositories;
using StudyBuddy.DAL.Repositories.Interfaces;
using StudyBuddy.Knowledge.Configuration;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Repositories;
using StudyBuddy.Knowledge.Services;
using StudyBuddy.Models.Confugrations;
using StudyBuddy.Processor.Factory;
using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Processors;
using StudyBuddy.Processor.Services;
using StudyBuddy.Ai.Services;
using StudyBuddy.Ai.Interfaces;
using System.Text;

namespace StudyBuddy.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Add services to the container.
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IRawDocumentRepository, RawDocumentRepository>();
            builder.Services.AddScoped<IStoredFileRepository, StoredFileRepository>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IRawDocumentService, RawDocumentService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddControllers();
            builder.Services.AddScoped<IProcesser, TextProcessor>();
            builder.Services.AddScoped<IProcessorFactory, ProcessorFactory>();
            builder.Services.AddScoped<IProcessingPipeline, ProcessingPipeline>();
            builder.Services.AddScoped<IProcesser, PdfProcessor>();
            builder.Services.AddScoped<IProcesser, SourceCodeProcessor>();
            builder.Services.AddScoped<ProcessingPipeline>();
            builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
            builder.Services.AddScoped<IImageOcrService, TesseractOcrService>();
            builder.Services.AddScoped<IProcesser, ImageProcessor>();
            builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
            builder.Services.AddScoped<IKnowledgeService, KnowledgeService>();
            builder.Services.AddScoped<IDetailedNotesRepository, DetailedNotesRepository>();
            builder.Services.AddScoped<IDetailedNotesService, DetailedNotesService>();
            builder.Services.AddScoped<IShortNotesRepository, ShortNotesRepository>();
            builder.Services.AddScoped<IShortNotesService, ShortNotesService>();

            builder.Services.AddHttpClient<IAiService, AiService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:8000");
                client.Timeout = TimeSpan.FromMinutes(10);   // 10 minutes
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });

                options.AddSecurityDefinition(AdminApiKeyAuthenticationHandler.SchemeName, new OpenApiSecurityScheme
                {
                    Name = AdminApiKeyAuthenticationHandler.HeaderName,
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Admin API key. Configure Admin:ApiKey (env: Admin__ApiKey)."
                });

                options.OperationFilter<AdminApiKeyOperationFilter>();
            });

            builder.Services.AddHttpClient<IAiService, AiService>(client =>
            {
                client.BaseAddress = new Uri(
                    builder.Configuration["AIService:BaseUrl"]!);
            });
            builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("Jwt"));

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp
                    .GetRequiredService<IOptions<MongoDbSettings>>()
                    .Value;

                return new MongoClient(settings.ConnectionString);
            });

            // Register of authentication 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,

                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],

                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                 };
             })
             .AddCookie()
             .AddGoogle(options =>
             {
                 options.ClientId =
                     builder.Configuration["Authentication:Google:ClientId"]!;

                 options.ClientSecret =
                     builder.Configuration["Authentication:Google:ClientSecret"]!;

                 options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
             });
            builder.Services
                .AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, AdminApiKeyAuthenticationHandler>(
                    AdminApiKeyAuthenticationHandler.SchemeName,
                    _ => { });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(AdminSeeder.AdminRole));
            });


            var app = builder.Build();

            await AdminSeeder.SeedAsync(
                app.Services,
                app.Configuration,
                app.Environment);

                        app.UseMiddleware<GlobalExceptionMiddleware>();
                        // Configure the HTTP request pipeline.
                      
             app.UseSwagger();
             app.UseSwaggerUI();


            // replace:  app.UseHttpsRedirection();
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            await app.RunAsync();
        }
    }
}

// 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