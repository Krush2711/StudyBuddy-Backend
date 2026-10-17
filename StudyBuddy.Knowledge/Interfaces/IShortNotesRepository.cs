using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IShortNotesRepository
    {
        Task CreateAsync(ShortNotesDocument document);
        Task<ShortNotesDocument?> GetByIdAsync(Guid id);
        Task<List<ShortNotesDocument>> GetByUserAsync(
            Guid userId,
            DateTime? fromUtc = null,
            string? subjectOrTag = null);
        Task DeleteAsync(Guid id);
        Task DeleteByUserAsync(Guid userId);
    }
}
