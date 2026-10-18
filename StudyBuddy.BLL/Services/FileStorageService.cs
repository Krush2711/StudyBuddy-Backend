
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using StudyBuddy.BLL.DTOs.Storage;
using StudyBuddy.BLL.Interfaces;

namespace StudyBuddy.BLL.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public FileStorageService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException(
                    "Cloudinary configuration is missing.");
            }

            var account = new Account(
                cloudName,
                apiKey,
                apiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<StoredFileResultDto> SaveAsync(
            Guid userId,
            Guid documentId,
            Stream stream,
            string fileName,
            string contentType)
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException(
                    "The file stream is empty or unreadable.",
                    nameof(stream));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentException(
                    "User ID is required.",
                    nameof(userId));
            }

            if (documentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Document ID is required.",
                    nameof(documentId));
            }

            var extension = Path.GetExtension(fileName);
            var storedFileName = $"{documentId}{extension}";

            // Each user's documents are stored in their own folder.
            var publicId =
                $"studybuddy/users/{userId}/documents/{documentId}{extension}";

            var fileSize = stream.CanSeek ? stream.Length - stream.Position : 0;

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = publicId,
                UseFilename = false,
                UniqueFilename = false,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new InvalidOperationException(
                    $"Cloudinary upload failed: {result.Error.Message}");
            }

            return new StoredFileResultDto
            {
                OriginalFileName = fileName,
                StoredFileName = storedFileName,
                StoragePath = result.SecureUrl?.ToString()
                    ?? result.PublicId,
                ContentType = contentType,
                FileExtension = extension,
                FileSize = fileSize
            };
        }

        public async Task DeleteAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var publicId = ExtractPublicId(filePath);

            if (string.IsNullOrWhiteSpace(publicId))
            {
                throw new InvalidOperationException(
                    "Could not determine the Cloudinary public ID.");
            }

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new InvalidOperationException(
                    $"Cloudinary deletion failed: {result.Error.Message}");
            }
        }

        private static string? ExtractPublicId(string filePath)
        {
            // Supports a stored Cloudinary public ID or a Cloudinary URL.
            if (!Uri.TryCreate(
                    filePath,
                    UriKind.Absolute,
                    out var uri))
            {
                return filePath;
            }

            var path = uri.AbsolutePath;
            var uploadIndex = path.IndexOf(
                "/upload/",
                StringComparison.OrdinalIgnoreCase);

            if (uploadIndex < 0)
                return null;

            var assetPath =
                path[(uploadIndex + "/upload/".Length)..];

            // Remove the Cloudinary version segment, such as v123456789/.
            var segments = assetPath.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length > 0 &&
                segments[0].StartsWith('v') &&
                segments[0].Length > 1 &&
                segments[0][1..].All(char.IsDigit))
            {
                assetPath = string.Join('/', segments.Skip(1));
            }

            // Preserve the extension for raw assets.
            return Uri.UnescapeDataString(assetPath);
        }
    }
}
