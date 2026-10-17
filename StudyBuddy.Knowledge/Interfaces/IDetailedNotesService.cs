using StudyBuddy.Ai.DTOs;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IDetailedNotesService
    {
        Task GenerateAsync(DocumentAnalysisRequestDto request, string? tag = null);
        Task<List<DetailedNotesDocument>> GetByUserAsync(
            Guid userId,
            DateTime? fromUtc = null,
            string? subjectOrTag = null);
        Task DeleteByUserAsync(Guid userId);
    }
}
