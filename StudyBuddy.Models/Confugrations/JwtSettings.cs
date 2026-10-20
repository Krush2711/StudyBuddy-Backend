using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Models.Confugrations
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Access-token lifetime. Kept short (15 min by default) because the
        /// refresh token now carries the session; a leaked access token is
        /// only useful for minutes.
        /// </summary>
        public int ExpiryInMinutes { get; set; } = 15;

        /// <summary>Refresh-token lifetime in days.</summary>
        public int RefreshTokenExpiryInDays { get; set; } = 30;
    }
}
