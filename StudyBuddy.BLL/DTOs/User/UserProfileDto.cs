namespace StudyBuddy.BLL.DTOs.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public string AuthenticationProvider { get; set; } = string.Empty;

        public bool IsEmailVerified { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}