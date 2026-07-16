using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyBuddy.Models.Entities;


namespace StudyBuddy.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey (x => x.Id);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.profileImageUrl).HasMaxLength(500);
            builder.Property(x => x.Isactive).HasDefaultValue(false);
            builder.Property(x => x.IsEmailVerified).HasDefaultValue(false);

            

        }
    }
}
