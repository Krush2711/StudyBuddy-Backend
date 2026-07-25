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
    }
}
