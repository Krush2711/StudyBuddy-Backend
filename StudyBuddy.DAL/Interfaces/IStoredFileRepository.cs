using StudyBuddy.Models.Entities;

namespace StudyBuddy.DAL.Interfaces
{
    public interface IStoredFileRepository
    {
        Task<StoredFile> AddAsync(StoredFile storedFile);
        Task<StoredFile?> GetByIdAsync(Guid id);
    }
}
