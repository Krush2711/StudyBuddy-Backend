using StudyBuddy.BLL.DTOs.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Interfaces
{
    public interface IFileStorageService
    {
        Task<StoredFileResultDto> SaveAsync(Stream stream,string fileName,string contentType);
    }
}
