using Microsoft.AspNetCore.DataProtection;
using StudyBuddy.Ai.Interfaces;
using System.Security.Cryptography;

namespace StudyBuddy.API.Security
{
    /// <summary>
    /// Encrypts API keys at rest with ASP.NET Core Data Protection.
    /// No secret is hardcoded; keys are managed by the Data Protection key ring.
    /// </summary>
    public sealed class DataProtectionGeminiApiKeyProtector : IGeminiApiKeyProtector
    {
        private readonly IDataProtector _protector;

        public DataProtectionGeminiApiKeyProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("StudyBuddy.GeminiApiKey.v1");
        }

        public string Protect(string apiKey)
        {
            return _protector.Protect(apiKey);
        }

        public string? Unprotect(string? protectedApiKey)
        {
            if (string.IsNullOrWhiteSpace(protectedApiKey))
            {
                return null;
            }

            try
            {
                return _protector.Unprotect(protectedApiKey);
            }
            catch (CryptographicException)
            {
                // Key ring changed or the stored value is corrupt: treat as missing.
                return null;
            }
        }
    }
}
