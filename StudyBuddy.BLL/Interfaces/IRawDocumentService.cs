using StudyBuddy.BLL.DTOs.RawDocument;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Interfaces
{
     public interface IRawDocumentService
    {
        Task<RawDocumentResponseDto> UploadAsync(Guid userId, UploadRawDocumentDto request);
    }
}
