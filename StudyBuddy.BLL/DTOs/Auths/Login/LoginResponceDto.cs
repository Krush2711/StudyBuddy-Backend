using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.Auths.Login
{
    public class LoginResponceDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
