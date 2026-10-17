using StudyBuddy.BLL.DTOs.Storage;

namespace StudyBuddy.BLL.Interfaces
{
    public interface IFileStorageService
    {
        Task<StoredFileResultDto> SaveAsync(
            Stream stream,
            string fileName,
            string contentType);

        Task DeleteAsync(string filePath);
    }
}
