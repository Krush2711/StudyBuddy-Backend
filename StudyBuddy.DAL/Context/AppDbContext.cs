using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models.Entities;


namespace StudyBuddy.DAL.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<StoredFile> StoredFiles { get; set; }

        public DbSet<RawDocument> RawDocuments { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity mappings and relationships here
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Refresh tokens are looked up by hash on every renewal, so the
            // index is on the hot path. Unique because a hash collision would
            // otherwise let one token resolve to two rows.
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(t => t.TokenHash).IsUnique();

                entity
                    .HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
