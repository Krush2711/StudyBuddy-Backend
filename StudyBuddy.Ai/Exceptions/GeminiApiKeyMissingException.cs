namespace StudyBuddy.Ai.Exceptions;

/// <summary>
/// Thrown when the authenticated user has not stored a Gemini API key yet.
/// </summary>
public class GeminiApiKeyMissingException : Exception
{
    public GeminiApiKeyMissingException()
        : base("No Gemini API key is configured for this account. " +
               "Save one with POST /api/users/me/gemini-key and try again.")
    {
    }
}
