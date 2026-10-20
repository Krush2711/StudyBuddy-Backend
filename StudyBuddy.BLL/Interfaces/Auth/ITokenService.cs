using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateToken(User user);

        /// <summary>
        /// The access-token lifetime in seconds, so the client can schedule its
        /// own refresh instead of waiting for a 401.
        /// </summary>
        int AccessTokenExpiresInSeconds { get; }

        /// <summary>Opaque, cryptographically random refresh token (raw value).</summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Hashes a raw refresh token for storage / lookup. The raw value is
        /// never persisted.
        /// </summary>
        string HashRefreshToken(string token);

        DateTime GetRefreshTokenExpiryUtc();
    }
}
