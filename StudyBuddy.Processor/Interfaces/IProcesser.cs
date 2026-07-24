using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Interfaces
{
    public interface IProcesser
    {
        bool CanProcess(string extension);
        Task<ProcessingResult> ProcessAsync(ProcessingContext context);
        
    }
}
