using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Interfaces
{
    public interface IDocumentProcessingService
    {
        Task<ProcessingResult> ProcessAsync(Guid rawDocumentId, string? tag = null);

    }
}
