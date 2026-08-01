using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.DTOs.Responce;
using StudyBuddy.Ai.Interfaces;
using System.Net.Http.Json;

namespace StudyBuddy.Ai.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
  

    public AiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DocumentAnalysisResponseDto> AnalyzeDocumentAsync(DocumentAnalysisRequestDto request)
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

            var summary = await response.Content.ReadFromJsonAsync<DocumentAnalysisResponseDto>();

            if (summary is null)
            {
                throw new Exception("Failed to deserialize AI Summary response.");
            }

            return summary;
        }

    }

        public async Task<DetailedNotesResponseDto> GenerateDetailedNotesAsync(DocumentAnalysisRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/notes",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"Detailed Notes generation failed.\n{error}");
            }

            var result =
                await response.Content.ReadFromJsonAsync<DetailedNotesResponseDto>();

            if (result == null)
                throw new Exception("Failed to deserialize Detailed Notes.");

            return result;
        }

    public async Task<ShortNotesResponseDto> GenerateShortNotesAsync(DocumentAnalysisRequestDto request)
         {
                var response = await _httpClient.PostAsJsonAsync(
                    "/shortnotes",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception(
                        $"Short Notes generation failed. Status {(int)response.StatusCode}. {error}");
        }

        var result =
            await response.Content.ReadFromJsonAsync<ShortNotesResponseDto>();

        if (result == null)
            throw new Exception("Unable to deserialize Short Notes response.");

        return result;
    }
}