using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class UploadRawDocumentDto
{
    [Required]
    public Stream FileStream { get; set; } = null!;
    [Required]
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string SourceType { get; set; } = string.Empty;
    [Required]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? LearningDate { get; set; }
}