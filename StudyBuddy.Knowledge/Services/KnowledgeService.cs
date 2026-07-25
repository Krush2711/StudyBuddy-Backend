using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;
using System;
using System.Collections.Generic;
using System.Text;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.Knowledge.Services
{
    public class KnowledgeService : IKnowledgeService
    {
        private readonly IKnowledgeRepository _repository;

        public KnowledgeService(IKnowledgeRepository repository)
        {
            _repository = repository;
        }

        public async Task StoreAsync(
            RawDocument rawDocument,
            ProcessingResult processingResult)
        {
            var knowledge = new KnowledgeDocument
            {
                //Id = Guid.NewGuid(),

                RawDocumentId = rawDocument.Id,

                UserId = rawDocument.UserId,

                FileName = rawDocument.StoredFile.OriginalFileName,

                FileType = rawDocument.StoredFile.FileExtension,

                ExtractedText = processingResult.ExtractedText,

                Metadata = processingResult.Metadata,

                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(knowledge);
        }
    }

}
