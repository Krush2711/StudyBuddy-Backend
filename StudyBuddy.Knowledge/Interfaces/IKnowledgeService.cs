using StudyBuddy.Knowledge.Models;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IKnowledgeService
    {
        Task StoreAsync(
            RawDocument rawDocument,
            ProcessingResult processingResult,
            string? tag = null);
        Task<List<KnowledgeDocument>> GetByUserAsync(
            Guid userId,
            DateTime? fromUtc = null,
            string? subjectOrTag = null);
        Task<KnowledgeDocument?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task DeleteByUserAsync(Guid userId);
    }
}
