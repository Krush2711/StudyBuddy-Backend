using StudyBuddy.Ai.Exceptions;
using StudyBuddy.Ai.Interfaces;
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
        private readonly IGeminiApiKeyProvider _geminiApiKeyProvider;

        public RawDocumentService(
            IRawDocumentRepository rawRepository,
            IStoredFileRepository storedFileRepository,
            IFileStorageService fileStorageService,
            IDocumentProcessingService documentProcessingService,
            IGeminiApiKeyProvider geminiApiKeyProvider)
        {
            _rawRepository = rawRepository;
            _storedFileRepository = storedFileRepository;
            _fileStorageService = fileStorageService;
            _documentProcessingService = documentProcessingService;
            _geminiApiKeyProvider = geminiApiKeyProvider;
        }


        public async Task<RawDocumentResponseDto> UploadAsync(
            Guid userId,
            UploadRawDocumentDto request)
        {
            if (request.FileStream == null)
                throw new Exception("File stream is empty.");

            var geminiApiKey =
                await _geminiApiKeyProvider.GetApiKeyAsync(userId);

            if (string.IsNullOrWhiteSpace(geminiApiKey))
                throw new GeminiApiKeyMissingException();

            // Generate one ID for both Cloudinary and RawDocument.
            var documentId = Guid.NewGuid();

            // Upload to the authenticated user's Cloudinary folder.
            var fileResult = await _fileStorageService.SaveAsync(
                userId,
                documentId,
                request.FileStream,
                request.FileName,
                request.ContentType);

            // Save file metadata.
            var storedFile = new StoredFile
            {
                Id = Guid.NewGuid(),
                OriginalFileName = fileResult.OriginalFileName,
                StoredFileName = fileResult.StoredFileName,
                StoragePath = fileResult.StoragePath,
                ContentType = fileResult.ContentType,
                FileExtension = fileResult.FileExtension,
                FileSize = fileResult.FileSize,
                StorageProvider = "Cloudinary",
                CreatedAt = DateTime.UtcNow
            };

            await _storedFileRepository.AddAsync(storedFile);

            // Create the document using the same ID as the Cloudinary asset.
            var rawDocument = new RawDocument
            {
                Id = documentId,
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

            await _documentProcessingService.ProcessAsync(
                rawDocument.Id,
                request.Subject);

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