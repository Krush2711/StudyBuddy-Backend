namespace StudyBuddy.Ai.Interfaces;

/// <summary>
/// Resolves the Gemini API key belonging to a user. Returns null when the user
/// has not saved a key (or when the stored value cannot be decrypted).
/// </summary>
public interface IGeminiApiKeyProvider
{
    Task<string?> GetApiKeyAsync(Guid userId);
}
