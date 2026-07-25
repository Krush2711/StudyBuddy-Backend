using StudyBuddy.Knowledge.Models;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IKnowledgeService
    {
        Task StoreAsync(RawDocument rawDocument, ProcessingResult processingResult);
        Task<List<KnowledgeDocument>> GetByUserAsync(Guid userId);
        Task<KnowledgeDocument?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
