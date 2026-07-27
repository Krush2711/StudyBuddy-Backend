using System.Net.Http.Json;

using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.Interfaces;

namespace StudyBuddy.Ai.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
  

    public AiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SummaryResponseDto> GenerateSummaryAsync(SummaryRequestDto request)
    {

        {
            var response = await _httpClient.PostAsJsonAsync(
                "/summary",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"AI Summary Generation Failed. Status: {(int)response.StatusCode}, Response: {error}");
            }

            var summary = await response.Content.ReadFromJsonAsync<SummaryResponseDto>();

            if (summary is null)
            {
                throw new Exception("Failed to deserialize AI Summary response.");
            }

            return summary;
        }

    }
}