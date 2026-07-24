using System;
using System.Collections.Generic;
using System.Text;
using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;

namespace StudyBuddy.Processor.Processors
{
    public class TextProcessor : IProcesser
    {
        public bool CanProcess(string extension)
        {
            return extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ProcessingResult> ProcessAsync(ProcessingContext context)
        {
            var text = await File.ReadAllTextAsync(context.FilePath);

            return new ProcessingResult
            {
                Success = true,
                ExtractedText = text,
            };
        }
    }
}
