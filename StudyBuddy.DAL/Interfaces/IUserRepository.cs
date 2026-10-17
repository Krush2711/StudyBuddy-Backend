using StudyBuddy.Models.Entities;

namespace StudyBuddy.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<List<User>> GetAllAsync();
        Task<List<RawDocument>> GetRawDocumentsByUserAsync(Guid userId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteAsync(Guid userId);
    }
}
