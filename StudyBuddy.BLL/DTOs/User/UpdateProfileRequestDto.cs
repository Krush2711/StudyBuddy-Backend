using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.User
{
    public class UpdateProfileRequestDto
    {
        public string fullName { get; set; } = string.Empty;
        public string profileImageUrl { get; set; } = string.Empty;
    }
}
