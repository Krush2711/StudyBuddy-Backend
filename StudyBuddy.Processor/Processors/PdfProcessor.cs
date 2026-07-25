using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using UglyToad.PdfPig;

namespace StudyBuddy.Processor.Processors
{
    public class PdfProcessor : IProcesser
    {
        public bool CanProcess(string extension)
        {
            return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ProcessingResult> ProcessAsync(ProcessingContext context)
        {
            return await Task.Run(() =>
            {
                var result = new ProcessingResult();

                try
                {
                    using var document = PdfDocument.Open(context.FilePath);
                    var textBuilder = new StringBuilder();
                    foreach (var page in document.GetPages())
                    {
                        textBuilder.AppendLine($"====================");
                        textBuilder.AppendLine($"Page {page.Number}");
                        textBuilder.AppendLine($"====================");
                        textBuilder.AppendLine();

                        textBuilder.AppendLine(page.Text);
                        textBuilder.AppendLine();

                    }

                    result.Success = true;
                    result.ExtractedText = textBuilder.ToString();
                    result.PageCount = document.NumberOfPages;
                    result.CharacterCount = result.ExtractedText.Length;

                    result.Metadata["Extension"] = ".pdf";
                    result.Metadata["ContentType"] = context.ContentType;

                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Error processing PDF: {ex.Message}";
                    return result;
                }
            });

        }
    }
}
