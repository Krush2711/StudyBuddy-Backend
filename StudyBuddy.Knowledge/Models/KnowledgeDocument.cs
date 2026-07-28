using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Knowledge.Models
{
    public class KnowledgeDocument
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }
        //[BsonRepresentation(BsonType.String)]

        public Guid UserId { get; set; }

        //[BsonRepresentation(BsonType.String)]
        

        public Guid RawDocumentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public string ExtractedText { get; set; } = string.Empty;

        public Dictionary<string, string> Metadata { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Summary { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string SubCategory { get; set; } = string.Empty;

        public string DocumentType { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public string Difficulty { get; set; } = string.Empty;

        public int EstimatedReadingTime { get; set; }

        public List<string> Topics { get; set; } = [];

        public List<string> Keywords { get; set; } = [];

        public List<string> Concepts { get; set; } = [];

        public List<string> Technologies { get; set; } = [];

        public List<string> LearningObjectives { get; set; } = [];

        public List<string> Prerequisites { get; set; } = [];

        public string RevisionPriority { get; set; } = string.Empty;

        public int ComplexityScore { get; set; }

        public double Confidence { get; set; }

        public bool HasCodeExamples { get; set; }

        public bool ContainsAlgorithm { get; set; }

        public bool ContainsFormula { get; set; }

        public bool ContainsDiagram { get; set; }
    }
}
