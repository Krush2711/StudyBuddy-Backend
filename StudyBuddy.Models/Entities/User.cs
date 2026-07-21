using StudyBuddy.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Models.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public int Id { get; set; } 
        public string? PasswordHash { get; set; } 

        public string? GoogleId { get; set; }
        public bool Isactive { get; set; } = true;

        public bool IsEmailVerified { get; set; } = false;
        public string? profileImageUrl { get; set; }

        public string AuthenticationProvider { get; set; } = "Local";

    }
}
