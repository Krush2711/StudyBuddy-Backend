namespace StudyBuddy.Processor.Models;

public class ProcessingResult
{
    public bool Success { get; set; }

    public string ExtractedText { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = new();

    public int PageCount { get; set; }

    public long CharacterCount { get; set; }

    public string? ErrorMessage { get; set; }

}