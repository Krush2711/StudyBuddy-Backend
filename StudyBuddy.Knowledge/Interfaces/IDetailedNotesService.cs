using StudyBuddy.Ai.DTOs;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IDetailedNotesService
    {
        Task GenerateAsync(DocumentAnalysisRequestDto request);
        Task<List<DetailedNotesDocument>> GetByUserAsync(Guid userId);
        Task DeleteByUserAsync(Guid userId);
    }
}
