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

        /// <summary>Exchanges a valid refresh token for a new session pair.</summary>
        Task<LoginResponceDto> RefreshAsync(string refreshToken);

        /// <summary>Revokes a refresh token. Safe to call with null/unknown values.</summary>
        Task LogoutAsync(string? refreshToken);
    }
}
