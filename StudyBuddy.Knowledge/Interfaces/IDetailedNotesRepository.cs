using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IDetailedNotesRepository
    {
        Task CreateAsync(DetailedNotesDocument document);
        Task<DetailedNotesDocument?> GetByRawDocumentIdAsync(Guid rawDocumentId);
        Task<List<DetailedNotesDocument>> GetByUserAsync(Guid userId);
        Task DeleteAsync(Guid id);
        Task DeleteByUserAsync(Guid userId);
    }
}
