using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StudyBuddy.Ai.Interfaces;
using StudyBuddy.Ai.Services;
using StudyBuddy.API.Authentication;
using StudyBuddy.API.Middleware;
using StudyBuddy.API.Security;
using StudyBuddy.API.Seeding;
using StudyBuddy.API.Swagger;
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
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;

namespace StudyBuddy.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString =
                    builder.Configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "PostgreSQL connection string is missing.");
                }

                options.UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsAssembly(
                        "StudyBuddy.DAL.PostgreSQL"));
            });
            // Add services to the container.
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IRawDocumentRepository, RawDocumentRepository>();
            builder.Services.AddScoped<IStoredFileRepository, StoredFileRepository>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IRawDocumentService, RawDocumentService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Gemini keys are encrypted at rest with Data Protection.
            builder.Services.AddDataProtection()
                .SetApplicationName("StudyBuddy");
            builder.Services.AddSingleton<IGeminiApiKeyProtector, DataProtectionGeminiApiKeyProtector>();
            builder.Services.AddScoped<IGeminiApiKeyProvider, GeminiApiKeyProvider>();

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
                client.Timeout = TimeSpan.FromMinutes(10);
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
             .AddCookie(options =>
             {
                 // The Google popup sets this cookie on the API's origin, and the
                 // SPA then reads it back with a cross-site fetch
                 // (credentials: 'include') in production. SameSite=Lax (the
                 // browser default) is NOT sent on cross-site fetches, which
                 // would leave the exchange returning 400 forever. None+Secure
                 // is required for that flow; dev is unaffected because the
                 // exchange goes through the same-origin Vite proxy there.
                 options.Cookie.SameSite = SameSiteMode.None;
                 options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
             })
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

            // CORS: the SPA calls this API from the browser, both locally (dev)
            // and on the deployed frontend (prod). AllowCredentials() is required
            // because the external (Google) sign-in round-trip relies on the auth
            // cookie — and it cannot be combined with AllowAnyOrigin(), so the
            // allowed origins are listed explicitly.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Spa", policy => policy
                    .WithOrigins(
                        "https://krushnas2.studybuddy.blitz.cloud",
                        "http://localhost:5173",
                        "http://localhost:5273")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            var app = builder.Build();

            await AdminSeeder.SeedAsync(
                app.Services,
                app.Configuration,
                app.Environment);

            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                // XForwardedHost lets Url/host-based redirects (e.g. the Google
                // sign-in redirect) resolve to the real public host behind the proxy
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            };
            forwardedHeadersOptions.KnownNetworks.Clear();   // ← still missing
            forwardedHeadersOptions.KnownProxies.Clear();

            // Blitz Cloud's proxy IP isn't known/fixed, so clear these
            // to allow forwarded headers from any upstream proxy

            forwardedHeadersOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardedHeadersOptions);
            app.UseCors("Spa");
            app.UseMiddleware<GlobalExceptionMiddleware>();
             // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


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