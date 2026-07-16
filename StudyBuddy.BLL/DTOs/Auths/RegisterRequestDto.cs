using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.Auths
{
    public sealed class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
