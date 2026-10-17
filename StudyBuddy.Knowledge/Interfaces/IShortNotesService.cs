using StudyBuddy.Ai.DTOs;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IShortNotesService
    {
        Task GenerateAsync(DocumentAnalysisRequestDto request);
        Task<List<ShortNotesDocument>> GetByUserAsync(Guid userId);
        Task DeleteByUserAsync(Guid userId);
    }
}
