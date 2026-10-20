namespace StudyBuddy.BLL.DTOs.Auths.Login
{
    /// <summary>
    /// The credential pair handed back by login, Google sign-in and refresh.
    /// `Token` is the short-lived access token; `RefreshToken` is the
    /// long-lived one used to mint the next access token.
    /// </summary>
    public class AuthTokensDto
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>Access-token lifetime in seconds, for the client's scheduler.</summary>
        public int ExpiresIn { get; set; }
    }
}
