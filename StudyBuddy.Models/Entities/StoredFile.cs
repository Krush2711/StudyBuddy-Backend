namespace StudyBuddy.Models.Entities
{
    public class StoredFile
    {
        public Guid Id { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string StoragePath { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public string FileExtension { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string? ContentHash { get; set; }

        public string StorageProvider { get; set; } = "Local";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}