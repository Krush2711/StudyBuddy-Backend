using StudyBuddy.Ai.DTOs;
using StudyBuddy.Models.Entities;
using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IDetailedNotesService
    {
        Task GenerateAsync(DocumentAnalysisRequestDto request);
    }
}