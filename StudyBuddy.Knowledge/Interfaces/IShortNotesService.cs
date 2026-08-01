using StudyBuddy.Ai.DTOs;

namespace StudyBuddy.Knowledge.Interfaces;

public interface IShortNotesService
{
    Task GenerateAsync(DocumentAnalysisRequestDto request);
}