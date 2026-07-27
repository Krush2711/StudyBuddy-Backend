using StudyBuddy.Ai.DTOs;

namespace StudyBuddy.Ai.Interfaces;

public interface IAiService
{
    Task<SummaryResponseDto> GenerateSummaryAsync(
        SummaryRequestDto request);
}