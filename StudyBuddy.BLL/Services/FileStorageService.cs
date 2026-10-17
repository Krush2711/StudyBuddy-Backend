using StudyBuddy.BLL.DTOs.Storage;
using StudyBuddy.BLL.Interfaces;

namespace StudyBuddy.BLL.Services
{
    public class FileStorageService : IFileStorageService
    {
        public async Task<StoredFileResultDto> SaveAsync(
            Stream stream,
            string fileName,
            string contentType)
        {
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Storage");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var storedFileName =
                $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            var filePath = Path.Combine(
                uploadsFolder,
                storedFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return new StoredFileResultDto
            {
                OriginalFileName = fileName,
                StoredFileName = storedFileName,
                StoragePath = filePath,
                ContentType = contentType,
                FileExtension = Path.GetExtension(fileName),
                FileSize = stream.Length
            };
        }

        public Task DeleteAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.CompletedTask;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
