using StudyBuddy.Models.Entities;

namespace StudyBuddy.DAL.Interfaces.Auth
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken token);

        /// <summary>Looks up a token by its hash, active or not.</summary>
        Task<RefreshToken?> GetByHashAsync(string tokenHash);

        Task UpdateAsync(RefreshToken token);

        /// <summary>Revokes every active token for a user (used on logout-all).</summary>
        Task<int> RevokeAllForUserAsync(Guid userId, string reason);
    }
}
