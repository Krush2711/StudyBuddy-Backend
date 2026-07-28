using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.DTOs.Responce;

namespace StudyBuddy.Ai.Interfaces;

public interface IAiService
{
    Task<DocumentAnalysisResponseDto> AnalyzeDocumentAsync(
    DocumentAnalysisRequestDto request);
}