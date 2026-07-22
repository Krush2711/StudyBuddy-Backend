using StudyBuddy.Models.Entities;

namespace StudyBuddy.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserAsync(User user);

    }
}