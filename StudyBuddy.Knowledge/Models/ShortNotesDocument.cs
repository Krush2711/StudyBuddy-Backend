using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudyBuddy.Knowledge.Models;

public class ShortNotesDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid RawDocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    // Optional user-supplied subject/tag from ingestion.
    public string Tag { get; set; } = string.Empty;

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

    public List<NoteSection> Sections { get; set; } = [];

    public DateTime CreatedAt { get; set; }
}