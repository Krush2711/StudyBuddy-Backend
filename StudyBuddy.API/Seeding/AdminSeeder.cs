using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.DAL.Context;
using StudyBuddy.Models.Entities;

namespace StudyBuddy.API.Seeding
{
    public static class AdminSeeder
    {
        public const string AdminRole = "Admin";

        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(AdminSeeder));
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply pending migrations automatically in development only.
            // In other environments, run `dotnet ef database update`.
            var pendingMigrations =
                (await db.Database.GetPendingMigrationsAsync()).ToList();

            if (pendingMigrations.Count > 0)
            {
                if (environment.IsDevelopment())
                {
                    logger.LogInformation(
                        "Applying {Count} pending migration(s): {Migrations}",
                        pendingMigrations.Count,
                        string.Join(", ", pendingMigrations));

                    await db.Database.MigrateAsync();
                }
                else
                {
                    logger.LogWarning(
                        "{Count} pending migration(s) were not applied automatically " +
                        "outside Development: {Migrations}",
                        pendingMigrations.Count,
                        string.Join(", ", pendingMigrations));
                }
            }

            var email = configuration["Admin:SeedEmail"];
            var password = configuration["Admin:SeedPassword"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                logger.LogInformation(
                    "Admin:SeedEmail / Admin:SeedPassword are not configured; " +
                    "skipping admin seeding.");
                return;
            }

            var passwordHasher = new PasswordHasher<User>();
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                user = new User
                {
                    FullName = "StudyBuddy Admin",
                    Email = email,
                    Role = AdminRole,
                    Isactive = true,
                    IsEmailVerified = true,
                    AuthenticationProvider = "Local"
                };

                user.PasswordHash =
                    passwordHasher.HashPassword(user, password);

                db.Users.Add(user);
                await db.SaveChangesAsync();

                logger.LogInformation(
                    "Seeded admin user {Email}.", email);
                return;
            }

            // Keep the seeded account usable as an admin.
            var changed = false;

            if (user.Role != AdminRole)
            {
                user.Role = AdminRole;
                changed = true;
            }

            if (!user.Isactive)
            {
                user.Isactive = true;
                changed = true;
            }

            if (!user.IsEmailVerified)
            {
                user.IsEmailVerified = true;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash =
                    passwordHasher.HashPassword(user, password);
                changed = true;
            }

            if (changed)
            {
                await db.SaveChangesAsync();
                logger.LogInformation(
                    "Updated seeded admin user {Email}.", email);
            }
        }
    }
}
