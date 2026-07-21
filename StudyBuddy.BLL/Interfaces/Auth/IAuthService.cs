using StudyBuddy.BLL.DTOs.Auths;
using StudyBuddy.BLL.DTOs.Auths.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponceDto> LoginAsync(LoginRequestDto request);

        Task<LoginResponceDto> GoogleLoginAsync(string googleId, string email,string name);
    }
}
