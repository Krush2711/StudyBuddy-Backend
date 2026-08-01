using StudyBuddy.BLL.DTOs.RawDocument;
using StudyBuddy.BLL.Interfaces;
using StudyBuddy.DAL.Interfaces;
using StudyBuddy.Models.Entities;

namespace StudyBuddy.BLL.Services
{
    public class RawDocumentService : IRawDocumentService
    {
        private readonly IRawDocumentRepository _rawRepository;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDocumentProcessingService _documentProcessingService;
        public RawDocumentService(
            IRawDocumentRepository rawRepository,
            IStoredFileRepository storedFileRepository,
            IFileStorageService fileStorageService,
            IDocumentProcessingService documentProcessingService)
        {
            _rawRepository = rawRepository;
            _storedFileRepository = storedFileRepository;
            _fileStorageService = fileStorageService;
            _documentProcessingService = documentProcessingService;
        }

        public async Task<RawDocumentResponseDto> UploadAsync(
            Guid userId,
            UploadRawDocumentDto request)
        {
            if (request.FileStream == null)
                throw new Exception("File stream is empty.");

            // Save physical file
            var fileResult = await _fileStorageService.SaveAsync(
                request.FileStream,
                request.FileName,
                request.ContentType);

            // Create StoredFile entity
            var storedFile = new StoredFile
            {
                Id = Guid.NewGuid(),
                OriginalFileName = fileResult.OriginalFileName,
                StoredFileName = fileResult.StoredFileName,
                StoragePath = fileResult.StoragePath,
                ContentType = fileResult.ContentType,
                FileExtension = fileResult.FileExtension,
                FileSize = fileResult.FileSize,
                StorageProvider = "Local",
                CreatedAt = DateTime.UtcNow
            };

            await _storedFileRepository.AddAsync(storedFile);

            // Create RawDocument
            var rawDocument = new RawDocument
            {
                Id = Guid.NewGuid(),
                UserId = userId,

                Title = string.IsNullOrWhiteSpace(request.Title)
                    ? Path.GetFileNameWithoutExtension(request.FileName)
                    : request.Title,

                Description = request.Description,

                SourceType = request.SourceType,

                StoredFileId = storedFile.Id,

                UploadedAt = DateTime.UtcNow,

                LearningDate = request.LearningDate ?? DateTime.UtcNow,

                ProcessingStatus = "Pending",

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = DateTime.UtcNow
            };

            await _rawRepository.AddAsync(rawDocument);
            await _documentProcessingService.ProcessAsync(rawDocument.Id);
            rawDocument.ProcessingStatus = "Completed";


            return new RawDocumentResponseDto
            {
                Id = rawDocument.Id,
                Title = rawDocument.Title,
                SourceType = rawDocument.SourceType,
                UploadedAt = rawDocument.UploadedAt,
                LearningDate = rawDocument.LearningDate,
                ProcessingStatus = rawDocument.ProcessingStatus
            };
        }
    }
}