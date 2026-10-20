using StudyBuddy.Ai.Interfaces;
using StudyBuddy.DAL.Repositories.Interfaces;

namespace StudyBuddy.BLL.Services
{
    /// <summary>
    /// Loads and decrypts a user's Gemini API key. Scoped, so the look-up is
    /// cached for the lifetime of a single request (ingestion makes several AI calls).
    /// </summary>
    public class GeminiApiKeyProvider : IGeminiApiKeyProvider
    {
        private readonly IUserRepository _userRepository;
        private readonly IGeminiApiKeyProtector _protector;
        private readonly Dictionary<Guid, string?> _cache = new();

        public GeminiApiKeyProvider(
            IUserRepository userRepository,
            IGeminiApiKeyProtector protector)
        {
            _userRepository = userRepository;
            _protector = protector;
        }


        public async Task<string?> GetApiKeyAsync(Guid userId)
        {
            // Read the user's currently stored plaintext key.
            var user = await _userRepository.GetUserByIdAsync(userId);

            var apiKey = user?.GeminiApiKey;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _cache.Remove(userId);
                return null;
            }

            return apiKey;
        }

    }
}
