using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.Models.Confugrations;
using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public string GenerateToken(User user)
        {
            // Implement token generation logic here using _jwtSettings

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
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
    }
}
//| Claim | Meaning |
//| ----- | --------------- |
//| Sub | User Id |
//| Email | User Email |
//| Name | User Name |
//| Jti | Unique Token Id |
