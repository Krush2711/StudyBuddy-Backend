using System.ComponentModel.DataAnnotations;

namespace StudyBuddy.BLL.DTOs.User
{
    public class SetGeminiApiKeyRequestDto
    {
        [Required(ErrorMessage = "Gemini API key is required.")]
        public string ApiKey { get; set; } = string.Empty;
    }
}
