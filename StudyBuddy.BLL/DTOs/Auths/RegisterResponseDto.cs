using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.Auths
{
    public sealed class RegisterResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; }
    }
}
