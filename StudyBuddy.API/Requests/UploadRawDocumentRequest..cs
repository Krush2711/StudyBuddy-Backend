using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StudyBuddy.API.Requests.RawDocument
{
    public class UploadRawDocumentRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public string SourceType { get; set; } = string.Empty;

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? LearningDate { get; set; }
    }
}