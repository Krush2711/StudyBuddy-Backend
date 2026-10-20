
using StudyBuddy.BLL.Interfaces;
using StudyBuddy.DAL.Interfaces;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.BLL.Services
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly IRawDocumentRepository _rawDocumentRepository;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IProcessingPipeline _processingPipeline;
        private readonly IKnowledgeService _knowledgeService;

        private static readonly HttpClient HttpClient = new();

        public DocumentProcessingService(
            IRawDocumentRepository rawDocumentRepository,
            IStoredFileRepository storedFileRepository,
            IProcessingPipeline processingPipeline,
            IKnowledgeService knowledgeService)
        {
            _rawDocumentRepository = rawDocumentRepository;
            _storedFileRepository = storedFileRepository;
            _processingPipeline = processingPipeline;
            _knowledgeService = knowledgeService;
        }

        public async Task<ProcessingResult> ProcessAsync(
            Guid rawDocumentId,
            string? tag = null)
        {
            var rawDocument =
                await _rawDocumentRepository.GetByIdAsync(rawDocumentId);

            if (rawDocument == null)
                throw new Exception("Raw document not found.");

            var storedFile =
                await _storedFileRepository.GetByIdAsync(
                    rawDocument.StoredFileId);

            if (storedFile == null)
                throw new Exception("Stored file not found.");

            string? temporaryFilePath = null;

            try
            {
                var filePath = storedFile.StoragePath;

                // Cloudinary URLs must be downloaded before local processors
                // such as PdfProcessor and SourceCodeProcessor can read them.
                if (Uri.TryCreate(
                        filePath,
                        UriKind.Absolute,
                        out var fileUri)
                    && (fileUri.Scheme == Uri.UriSchemeHttp
                        || fileUri.Scheme == Uri.UriSchemeHttps))
                {
                    var extension = storedFile.FileExtension;

                    if (string.IsNullOrWhiteSpace(extension))
                    {
                        extension = Path.GetExtension(fileUri.AbsolutePath);
                    }

                    if (string.IsNullOrWhiteSpace(extension))
                    {
                        throw new InvalidOperationException(
                            "Could not determine the downloaded file extension.");
                    }

                    var temporaryDirectory = Path.Combine(
                        Path.GetTempPath(),
                        "StudyBuddyProcessing");

                    Directory.CreateDirectory(temporaryDirectory);

                    temporaryFilePath = Path.Combine(
                        temporaryDirectory,
                        $"{Guid.NewGuid()}{extension}");

                    using var response = await HttpClient.GetAsync(
                        fileUri,
                        HttpCompletionOption.ResponseHeadersRead);

                    response.EnsureSuccessStatusCode();

                    await using (var destination = new FileStream(
                        temporaryFilePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        await response.Content.CopyToAsync(destination);
                    }

                    filePath = temporaryFilePath;
                }
                else if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(
                        "The stored file does not exist.",
                        filePath);
                }

                var context = new ProcessingContext
                {
                    RawDocumentId = rawDocument.Id,
                    FilePath = filePath,
                    FileExtension = storedFile.FileExtension,
                    ContentType = storedFile.ContentType
                };

                var result =
                    await _processingPipeline.ProcessAsync(context);

                if (result.Success)
                {
                    rawDocument.StoredFile = storedFile;

                    await _knowledgeService.StoreAsync(
                        rawDocument,
                        result,
                        tag);
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
            catch
            {
                rawDocument.ProcessingStatus = "Failed";
                rawDocument.UpdatedAt = DateTime.UtcNow;

                await _rawDocumentRepository.UpdateAsync(rawDocument);

                throw;
            }
            finally
            {
                // Remove downloaded temporary files, but never delete
                // the original file or the Cloudinary asset.
                if (temporaryFilePath != null
                    && File.Exists(temporaryFilePath))
                {
                    try
                    {
                        File.Delete(temporaryFilePath);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(
                            $"Temporary file cleanup failed: {ex.Message}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine(
                            $"Temporary file cleanup failed: {ex.Message}");
                    }
                }
            }
        }
    }
}
