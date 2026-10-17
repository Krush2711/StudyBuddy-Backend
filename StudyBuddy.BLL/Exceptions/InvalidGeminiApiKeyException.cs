namespace StudyBuddy.BLL.Exceptions;

/// <summary>
/// Thrown when a supplied Gemini API key is empty or not in the expected format.
/// </summary>
public class InvalidGeminiApiKeyException : Exception
{
    public InvalidGeminiApiKeyException(string message) : base(message)
    {
    }
}
