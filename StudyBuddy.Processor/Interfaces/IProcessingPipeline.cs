using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Interfaces
{
    public interface IProcessingPipeline
    {
        Task <ProcessingResult> ProcessAsync(ProcessingContext context);
    }
}
