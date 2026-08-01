using System;
using System.Collections.Generic;
using System.Text;
using StudyBuddy.Ai.DTOs;

namespace StudyBuddy.Ai.DTOs.Responce
{
    public class DetailedNotesResponseDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RawDocumentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Overview { get; set; } = string.Empty;

        public List<NoteSectionDto> Sections { get; set; } = new();

        public List<string> ImportantPoints { get; set; } = new();

        public List<string> Examples { get; set; } = new();

        public List<string> BestPractices { get; set; } = new();

        public List<string> CommonMistakes { get; set; } = new();

        public List<string> RevisionTips { get; set; } = new();
    }
}
