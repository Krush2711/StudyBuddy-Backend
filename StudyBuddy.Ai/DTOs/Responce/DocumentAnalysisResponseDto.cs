using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Ai.DTOs.Responce
{
    public class DocumentAnalysisResponseDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RawDocumentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        // AI Summary
        public string Summary { get; set; } = string.Empty;

        // Document Information
        public string Subject { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string SubCategory { get; set; } = string.Empty;

        public string DocumentType { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        // Learning Information
        public string Difficulty { get; set; } = string.Empty;

        public int EstimatedReadingTime { get; set; }

        public List<string> LearningObjectives { get; set; } = [];

        public List<string> Prerequisites { get; set; } = [];

        // Knowledge Information
        public List<string> Topics { get; set; } = [];

        public List<string> Keywords { get; set; } = [];

        public List<string> Concepts { get; set; } = [];

        public List<string> Technologies { get; set; } = [];

        // AI Intelligence
        public string RevisionPriority { get; set; } = string.Empty;

        public int ComplexityScore { get; set; }

        public double Confidence { get; set; }

        // Content Detection
        public bool HasCodeExamples { get; set; }

        public bool ContainsAlgorithm { get; set; }

        public bool ContainsFormula { get; set; }

        public bool ContainsDiagram { get; set; }
    }
}
