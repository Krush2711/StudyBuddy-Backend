using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.Processor.Processors;

public class ImageProcessor : IProcesser
{
    private readonly IImageOcrService _ocrService;

    public ImageProcessor(IImageOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    private static readonly HashSet<string> SupportedExtensions =
    [
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".gif",
        ".tiff",
        ".webp"
    ];

    public bool CanProcess(string extension)
    {
        return SupportedExtensions.Contains(extension.ToLowerInvariant());
    }

    public async Task<ProcessingResult> ProcessAsync(
        ProcessingContext context)
    {
        var result = new ProcessingResult();

        try
        {
            var text = await _ocrService.ExtractTextAsync(context.FilePath);

            result.Success = true;
            result.ExtractedText = text;
            result.CharacterCount = text.Length;
            result.PageCount = 1;

            result.Metadata["Processor"] = "ImageProcessor";
            result.Metadata["Extension"] = context.FileExtension;
            result.Metadata["ContentType"] = context.ContentType;
            result.Metadata["FileName"] = Path.GetFileName(context.FilePath);

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;

            return result;
        }
    }
}