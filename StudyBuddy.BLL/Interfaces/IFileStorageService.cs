using StudyBuddy.BLL.DTOs.Storage;

namespace StudyBuddy.BLL.Interfaces
{
    public interface IFileStorageService
    {
        Task<StoredFileResultDto> SaveAsync(Guid userId,
       Guid documentId,
       Stream stream,
       string fileName,
       string contentType);
        Task DeleteAsync(string filePath);
    }
}
