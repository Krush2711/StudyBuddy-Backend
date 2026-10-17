using StudyBuddy.Ai.Interfaces;
using StudyBuddy.BLL.DTOs.Admin;
using StudyBuddy.BLL.DTOs.User;
using StudyBuddy.BLL.Exceptions;
using StudyBuddy.BLL.Interfaces;
using StudyBuddy.DAL.Repositories.Interfaces;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;
using StudyBuddy.Models.Entities;
using System.Text.RegularExpressions;

namespace StudyBuddy.BLL.Services
{
    public class UserService : IUserService
    {
        // Google AI Studio keys start with "AIza" followed by ~35 URL-safe characters.
        private static readonly Regex GeminiApiKeyPattern = new(
            @"^AIza[0-9A-Za-z\-_]{30,60}$",
            RegexOptions.Compiled);

        private readonly IUserRepository _userRepository;
        private readonly IKnowledgeService _knowledgeService;
        private readonly IDetailedNotesService _detailedNotesService;
        private readonly IShortNotesService _shortNotesService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IGeminiApiKeyProtector _geminiApiKeyProtector;

        public UserService(
            IUserRepository userRepository,
            IKnowledgeService knowledgeService,
            IDetailedNotesService detailedNotesService,
            IShortNotesService shortNotesService,
            IFileStorageService fileStorageService,
            IGeminiApiKeyProtector geminiApiKeyProtector)
        {
            _userRepository = userRepository;
            _knowledgeService = knowledgeService;
            _detailedNotesService = detailedNotesService;
            _shortNotesService = shortNotesService;
            _fileStorageService = fileStorageService;
            _geminiApiKeyProtector = geminiApiKeyProtector;
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

        public async Task SaveGeminiApiKeyAsync(
            Guid userId,
            SetGeminiApiKeyRequestDto request)
        {
            var apiKey = request.ApiKey?.Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidGeminiApiKeyException(
                    "Gemini API key is required.");
            }

            // Basic format validation before calling Google. The key is verified
            //// for real on the next AI request; rejected keys return a clear 400.
            //if (!GeminiApiKeyPattern.IsMatch(apiKey))
            //{
            //    throw new InvalidGeminiApiKeyException(
            //        "Invalid Gemini API key format. Expected a key beginning with \"AIza\".");
            //}

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            // Encrypted at rest; the plaintext key is never persisted or returned.
            user.GeminiApiKey = _geminiApiKeyProtector.Protect(apiKey);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(MapToAdminDto).ToList();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            var rawDocuments =
                await _userRepository.GetRawDocumentsByUserAsync(userId);

            var filePaths = rawDocuments
                .Where(x => x.StoredFile != null)
                .Select(x => x.StoredFile!.StoragePath)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            // MongoDB and local files are not covered by the SQL cascade.
            await _knowledgeService.DeleteByUserAsync(userId);
            await _detailedNotesService.DeleteByUserAsync(userId);
            await _shortNotesService.DeleteByUserAsync(userId);

            foreach (var filePath in filePaths)
            {
                await _fileStorageService.DeleteAsync(filePath);
            }

            if (!await _userRepository.DeleteAsync(userId))
            {
                throw new UserNotFoundException(userId);
            }
        }

        public async Task<List<KnowledgeDocument>> GetUserKnowledgeAsync(
            Guid userId)
        {
            await EnsureUserExistsAsync(userId);

            return await _knowledgeService.GetByUserAsync(userId);
        }

        public async Task<AdminUserNotesDto> GetUserNotesAsync(Guid userId)
        {
            await EnsureUserExistsAsync(userId);

            var detailedNotes =
                await _detailedNotesService.GetByUserAsync(userId);
            var shortNotes =
                await _shortNotesService.GetByUserAsync(userId);

            return new AdminUserNotesDto
            {
                UserId = userId,
                DetailedNotes = detailedNotes,
                ShortNotes = shortNotes
            };
        }

        private async Task EnsureUserExistsAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
        }

        private static AdminUserDto MapToAdminDto(User user)
        {
            return new AdminUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AuthenticationProvider = user.AuthenticationProvider,
                Role = user.Role,
                IsActive = user.Isactive,
                IsEmailVerified = user.IsEmailVerified,
                ProfileImageUrl = user.profileImageUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}