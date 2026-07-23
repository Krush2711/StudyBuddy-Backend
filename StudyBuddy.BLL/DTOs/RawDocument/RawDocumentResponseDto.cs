using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.RawDocument
{
    public class RawDocumentResponseDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string SourceType { get; set; } = string.Empty;

        public string ProcessingStatus { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public DateTime LearningDate { get; set; }
    }
}
