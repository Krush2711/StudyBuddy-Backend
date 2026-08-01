using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Ai.DTOs.Responce
{
    public class ShortNotesResponseDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RawDocumentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public List<string> KeyConcepts { get; set; } = [];

        public List<string> ImportantDefinitions { get; set; } = [];

        public List<string> Formulas { get; set; } = [];

        public List<string> Syntax { get; set; } = [];

        public List<string> ImportantPoints { get; set; } = [];

        public List<string> InterviewTips { get; set; } = [];

        public List<string> RevisionChecklist { get; set; } = [];

        public List<string> Mnemonics { get; set; } = [];

        public List<NoteSectionDto> Sections { get; set; } = [];
    }
}
