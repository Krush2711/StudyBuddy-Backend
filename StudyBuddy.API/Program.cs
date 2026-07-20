using Microsoft.EntityFrameworkCore;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.BLL.Services;
using StudyBuddy.BLL.Services.Add;
using StudyBuddy.DAL.Context;
using StudyBuddy.DAL.Interfaces.Auth;
using StudyBuddy.DAL.Repositories;
using StudyBuddy.API.Middleware;

namespace StudyBuddy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Add services to the container.
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseAuthorization();

            
            app.MapControllers();

            app.Run();
        }
    }
}
