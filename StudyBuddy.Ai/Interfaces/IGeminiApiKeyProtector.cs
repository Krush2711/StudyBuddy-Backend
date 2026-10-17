namespace StudyBuddy.Ai.Interfaces;

/// <summary>
/// Encrypts Gemini API keys before they are persisted and decrypts them on use.
/// </summary>
public interface IGeminiApiKeyProtector
{
    string Protect(string apiKey);

    string? Unprotect(string? protectedApiKey);
}
