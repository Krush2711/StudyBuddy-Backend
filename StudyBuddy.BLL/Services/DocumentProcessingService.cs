using StudyBuddy.BLL.Interfaces;
using StudyBuddy.DAL.Interfaces;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Services;
using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Services
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly IRawDocumentRepository _rawDocumentRepository;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IProcessingPipeline _processingPipeline;
        private readonly IKnowledgeService _knowledgeService;

        public DocumentProcessingService(IRawDocumentRepository rawDocumentRepository, IStoredFileRepository storedFileRepository, IProcessingPipeline processingPipeline, IKnowledgeService knowledgeService)
        {
            _rawDocumentRepository = rawDocumentRepository;
            _storedFileRepository = storedFileRepository;
            _processingPipeline = processingPipeline;
            _knowledgeService = knowledgeService;

        }

        public async Task<ProcessingResult> ProcessAsync(Guid rawDocumentId, string? tag = null)
        {
            // Get Raw Document
            var rawDocument = await _rawDocumentRepository.GetByIdAsync(rawDocumentId);

            if (rawDocument == null)
                throw new Exception("Raw document not found.");

            // Get Stored File
            var storedFile = await _storedFileRepository.GetByIdAsync(rawDocument.StoredFileId);

            if (storedFile == null)
                throw new Exception("Stored file not found.");

            // Build Processing Context
            try
            {
                var context = new ProcessingContext
                {
                    RawDocumentId = rawDocument.Id,
                    FilePath = storedFile.StoragePath,
                    FileExtension = storedFile.FileExtension,
                    ContentType = storedFile.ContentType
                };

                var result = await _processingPipeline.ProcessAsync(context);
                if (result.Success)
                {
                    rawDocument.StoredFile = storedFile; // Since you loaded the file separately, you're simply assigning it to the navigation property before passing the document to the service.

                    await _knowledgeService.StoreAsync(rawDocument, result, tag);
                }
                Console.WriteLine("========== Extracted Text ==========");
                Console.WriteLine(result.ExtractedText);
                Console.WriteLine("====================================");

                rawDocument.ProcessingStatus = result.Success
                    ? "Completed"
                    : "Failed";

                rawDocument.UpdatedAt = DateTime.UtcNow;

                await _rawDocumentRepository.UpdateAsync(rawDocument);

                return result;
            }
            catch (Exception ex)
            {
                rawDocument.ProcessingStatus = "Failed";
                rawDocument.UpdatedAt = DateTime.UtcNow;

                await _rawDocumentRepository.UpdateAsync(rawDocument);

                throw;
            }

        }
    }

}
