using StudyBuddy.Knowledge.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IKnowledgeRepository
    {
        Task<KnowledgeDocument> CreateAsync(KnowledgeDocument document);

        Task<KnowledgeDocument?> GetByIdAsync(Guid id);
        Task UpdateSummaryAsync(Guid rawDocumentId, string summary);

        Task<List<KnowledgeDocument>> GetByUserAsync(Guid userId);

        Task DeleteAsync(Guid id);
    }
}
