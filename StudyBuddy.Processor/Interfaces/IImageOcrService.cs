using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Interfaces
{
    public interface IImageOcrService
    {
        Task<string> ExtractTextAsync(string imagePath);
    }
}
