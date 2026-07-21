using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.Auths.Login
{
    public class LoginRequestDto
    {

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
