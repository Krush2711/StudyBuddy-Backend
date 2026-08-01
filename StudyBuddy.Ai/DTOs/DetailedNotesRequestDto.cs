using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Ai.DTOs
{
    public class DetailedNotesRequestDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RawDocumentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public string ExtractedText { get; set; } = string.Empty;
    }
}
