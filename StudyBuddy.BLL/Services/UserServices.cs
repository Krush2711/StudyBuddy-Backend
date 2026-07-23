using StudyBuddy.BLL.DTOs.User;
using StudyBuddy.BLL.Exceptions;
using StudyBuddy.BLL.Interfaces;
using StudyBuddy.DAL.Repositories.Interfaces;

namespace StudyBuddy.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfileImageUrl = user.profileImageUrl,
                AuthenticationProvider = user.AuthenticationProvider,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            user.FullName = request.fullName;
            user.profileImageUrl = request.profileImageUrl;

            await _userRepository.UpdateUserAsync(user);
            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfileImageUrl = user.profileImageUrl,
                AuthenticationProvider = user.AuthenticationProvider,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            };

        }
    }
}