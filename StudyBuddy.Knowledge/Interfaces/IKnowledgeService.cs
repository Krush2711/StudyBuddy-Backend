using StudyBuddy.Knowledge.Models;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IKnowledgeService
    {
        Task StoreAsync(RawDocument rawDocument, ProcessingResult processingResult);
        Task<List<KnowledgeDocument>> GetByUserAsync(Guid userId);
        Task<KnowledgeDocument?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task DeleteByUserAsync(Guid userId);
    }
}
