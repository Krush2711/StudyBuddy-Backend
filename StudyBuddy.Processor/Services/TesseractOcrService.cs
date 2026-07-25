using StudyBuddy.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Tesseract;

namespace StudyBuddy.Processor.Services
{
    public class TesseractOcrService : IImageOcrService
    {
        public async Task<string> ExtractTextAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);

                using var image = Pix.LoadFromFile(imagePath);

                using var page = engine.Process(image);

                return page.GetText();
            });

        }
    }
}
