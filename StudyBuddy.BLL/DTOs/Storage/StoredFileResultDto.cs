using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.DTOs.Storage
{
    public  class StoredFileResultDto
    {
        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string StoragePath { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public string FileExtension { get; set; } = string.Empty;

        public long FileSize { get; set; }
    }
}
