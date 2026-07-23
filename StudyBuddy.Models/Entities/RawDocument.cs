using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Models.Entities
{
    public class RawDocument
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string SourceType { get; set; } = string.Empty;

        public Guid StoredFileId { get; set; }

        public StoredFile StoredFile { get; set; } = null!;

        public DateTime UploadedAt { get; set; }

        public DateTime LearningDate { get; set; }

        public string ProcessingStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
