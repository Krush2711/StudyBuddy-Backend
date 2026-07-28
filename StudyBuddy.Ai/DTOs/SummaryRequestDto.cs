namespace StudyBuddy.Ai.DTOs;

public class DocumentAnalysisRequestDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid RawDocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string ExtractedText { get; set; } = string.Empty;
}