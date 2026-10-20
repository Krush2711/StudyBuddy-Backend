using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.Models.Confugrations;
using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml;


namespace StudyBuddy.BLL.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> options) { 
                        _jwtSettings = options.Value;
        }

        public int AccessTokenExpiresInSeconds =>
            Math.Max(1, _jwtSettings.ExpiryInMinutes) * 60;

        public string GenerateToken(User user)
        {
            // Implement token generation logic here using _jwtSettings

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /// <summary>
        /// 256 bits from a CSPRNG, Base64url-encoded. Deliberately opaque — it
        /// carries no claims, so it can be revoked server-side, which a JWT
        /// cannot be.
        /// </summary>
        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public string HashRefreshToken(string token)
        {
            // SHA-256 is right here (not a password hash): the input is already
            // high-entropy random, so slow KDFs would only add latency.
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));

            return Convert.ToBase64String(hash);
        }

        public DateTime GetRefreshTokenExpiryUtc()
        {
            var days = Math.Max(1, _jwtSettings.RefreshTokenExpiryInDays);

            return DateTime.UtcNow.AddDays(days);
        }
    }
}
//| Claim | Meaning |
//| ----- | --------------- |
//| Sub | User Id |
//| Email | User Email |
//| Name | User Name |
//| Jti | Unique Token Id |
