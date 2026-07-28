using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.Interfaces;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.Knowledge.Services
{
    public class KnowledgeService : IKnowledgeService
    {
        private readonly IKnowledgeRepository _repository;
        private readonly IAiService _aiService;

        public KnowledgeService(
            IKnowledgeRepository repository,
            IAiService aiService)
        {
            _repository = repository;
            _aiService = aiService;
        }

        public async Task StoreAsync(
            RawDocument rawDocument,
            ProcessingResult processingResult)
        {
            // Create Knowledge Document
            var knowledge = new KnowledgeDocument
            {
                RawDocumentId = rawDocument.Id,
                UserId = rawDocument.UserId,
                FileName = rawDocument.StoredFile.OriginalFileName,
                FileType = rawDocument.StoredFile.FileExtension,
                ExtractedText = processingResult.ExtractedText,
                Metadata = processingResult.Metadata,
                CreatedAt = DateTime.UtcNow
            };

            // Save into MongoDB
            var createdDocument = await _repository.CreateAsync(knowledge);

            // Call AI Service
            var analysis = await _aiService.AnalyzeDocumentAsync(
                new DocumentAnalysisRequestDto
                {
                    Id = createdDocument.Id,
                    UserId = createdDocument.UserId,
                    RawDocumentId = createdDocument.RawDocumentId,
                    FileName = createdDocument.FileName,
                    FileType = createdDocument.FileType,
                    ExtractedText = createdDocument.ExtractedText
                });

            // Update AI Analysis
            await _repository.UpdateAnalysisAsync(
                createdDocument.RawDocumentId,
                analysis);
        }

        public async Task<List<KnowledgeDocument>> GetByUserAsync(Guid userId)
        {
            return await _repository.GetByUserAsync(userId);
        }

        public async Task<KnowledgeDocument?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}