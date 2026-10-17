namespace StudyBuddy.Ai.Exceptions;

/// <summary>
/// Thrown when Google rejects the stored Gemini API key (invalid, revoked or unauthorized).
/// </summary>
public class GeminiApiKeyRejectedException : Exception
{
    public GeminiApiKeyRejectedException()
        : base("The stored Gemini API key was rejected by Google. " +
               "Verify the key and save it again with POST /api/users/me/gemini-key.")
    {
    }
}
