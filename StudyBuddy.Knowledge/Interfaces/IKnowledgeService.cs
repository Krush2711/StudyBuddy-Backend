using System;
using System.Collections.Generic;
using System.Text;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;


namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IKnowledgeService
    {
        Task StoreAsync(RawDocument rawDocument, ProcessingResult processingResult);
    }
}
