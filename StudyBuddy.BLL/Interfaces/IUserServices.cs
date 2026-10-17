using StudyBuddy.BLL.DTOs.Admin;
using StudyBuddy.BLL.DTOs.User;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
        Task<List<AdminUserDto>> GetAllUsersAsync();
        Task DeleteUserAsync(Guid userId);
        Task<List<KnowledgeDocument>> GetUserKnowledgeAsync(Guid userId);
        Task<AdminUserNotesDto> GetUserNotesAsync(Guid userId);
    }
}
