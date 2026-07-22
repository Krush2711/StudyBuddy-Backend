using StudyBuddy.BLL.DTOs.User;
using StudyBuddy.Models.Entities;

namespace StudyBuddy.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
    }
}