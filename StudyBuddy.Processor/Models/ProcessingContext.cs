using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Models
{
    public class ProcessingContext
    {
        public Guid RawDocumentId { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public string FileExtension { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;
    }
}
