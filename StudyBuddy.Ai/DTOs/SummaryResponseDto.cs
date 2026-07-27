namespace StudyBuddy.Ai.DTOs;

public class SummaryResponseDto
{
    public Guid Id { get; set; }

    public Guid RawDocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;
}