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
        private readonly IDetailedNotesService _detailedNotesService;
        private readonly IShortNotesService _shortNotesService;

        public KnowledgeService(
            IKnowledgeRepository repository,
            IAiService aiService,
            IDetailedNotesService detailedNotesService,
            IShortNotesService shortNotesService)
        {
            _repository = repository;
            _aiService = aiService;
            _detailedNotesService = detailedNotesService;
            _shortNotesService = shortNotesService;
        }

        public async Task StoreAsync(
            RawDocument rawDocument,
            ProcessingResult processingResult)
        {
            // STEP 1 : Create Knowledge Document
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

            // STEP 2 : Save Knowledge Document
            var createdDocument = await _repository.CreateAsync(knowledge);

            // STEP 3 : Build AI Request
            var request = new DocumentAnalysisRequestDto
            {
                Id = createdDocument.Id,
                UserId = createdDocument.UserId,
                RawDocumentId = createdDocument.RawDocumentId,
                FileName = createdDocument.FileName,
                FileType = createdDocument.FileType,
                ExtractedText = createdDocument.ExtractedText
            };

            // STEP 4 : Generate Summary & Metadata
            var analysis = await _aiService.AnalyzeDocumentAsync(request);

            // STEP 5 : Update Knowledge Document
            await _repository.UpdateAnalysisAsync(
                createdDocument.RawDocumentId,
                analysis);

            // STEP 6 : Generate Detailed Notes
            await _detailedNotesService.GenerateAsync(request);

            await _shortNotesService.GenerateAsync(request);
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

        public async Task DeleteByUserAsync(Guid userId)
        {
            await _repository.DeleteByUserAsync(userId);
        }

    }
}