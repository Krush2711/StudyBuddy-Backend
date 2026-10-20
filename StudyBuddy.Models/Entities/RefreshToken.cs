using StudyBuddy.Models.Common;

namespace StudyBuddy.Models.Entities
{
    /// <summary>
    /// A long-lived credential that exchanges for a new access token.
    ///
    /// Only the SHA-256 hash of the token is persisted, so a database leak does
    /// not hand out usable sessions. Each row belongs to one login and is
    /// rotated on every use: the old row is revoked the moment a new one is
    /// issued, which is what makes replay detectable.
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }

        /// <summary>Base64 SHA-256 of the raw token. Never the token itself.</summary>
        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// The token that replaced this one during rotation, so a replayed
        /// (already-used) token can be traced to the family it belongs to.
        /// </summary>
        public Guid? ReplacedByTokenId { get; set; }

        /// <summary>Free-text reason kept for auditing: "rotated", "logout", …</summary>
        public string? RevokedReason { get; set; }

        public User? User { get; set; }

        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    }
}
