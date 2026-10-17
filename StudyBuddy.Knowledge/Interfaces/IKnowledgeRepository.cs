using StudyBuddy.Ai.DTOs.Responce;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IKnowledgeRepository
    {
        Task<KnowledgeDocument> CreateAsync(KnowledgeDocument document);
        Task<KnowledgeDocument?> GetByIdAsync(Guid id);
        Task UpdateAnalysisAsync(Guid rawDocumentId, DocumentAnalysisResponseDto analysis);
        Task<List<KnowledgeDocument>> GetByUserAsync(Guid userId);
        Task DeleteAsync(Guid id);
        Task DeleteByUserAsync(Guid userId);
    }
}
