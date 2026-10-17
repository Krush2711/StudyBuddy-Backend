using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace StudyBuddy.API.Authentication
{
    public sealed class AdminApiKeyAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "AdminApiKey";
        public const string HeaderName = "X-Admin-Api-Key";
        public const string ConfigKey = "Admin:ApiKey";

        private readonly IConfiguration _configuration;
        public AdminApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var headerValues))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Admin API key was not supplied."));
            }

            var providedKey = headerValues.ToString();
            var expectedKey = _configuration[ConfigKey];

            if (string.IsNullOrWhiteSpace(providedKey) ||
                string.IsNullOrWhiteSpace(expectedKey))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Admin API key is invalid."));
            }

            if (!FixedTimeEquals(providedKey, expectedKey))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Admin API key is invalid."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static bool FixedTimeEquals(string provided, string expected)
        {
            var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
            var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));

            return CryptographicOperations.FixedTimeEquals(
                providedHash,
                expectedHash);
        }
    }
}
