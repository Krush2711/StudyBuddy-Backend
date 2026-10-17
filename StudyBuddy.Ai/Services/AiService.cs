using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.DTOs.Responce;
using StudyBuddy.Ai.Exceptions;
using StudyBuddy.Ai.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace StudyBuddy.Ai.Services;

public class AiService : IAiService
{
    // Passed to the FastAPI AI service, which forwards it to Google.
    // Never written to logs or responses.
    private const string GeminiApiKeyHeader = "X-Gemini-Api-Key";

    private readonly HttpClient _httpClient;
    private readonly IGeminiApiKeyProvider _apiKeyProvider;

    public AiService(
        HttpClient httpClient,
        IGeminiApiKeyProvider apiKeyProvider)
    {
        _httpClient = httpClient;
        _apiKeyProvider = apiKeyProvider;
    }

    public async Task<DocumentAnalysisResponseDto> AnalyzeDocumentAsync(
        DocumentAnalysisRequestDto request)
    {
        using var response = await PostAsync("/summary", request);

        await EnsureSuccessAsync(response, "AI Summary Generation");

        var summary = await response.Content
            .ReadFromJsonAsync<DocumentAnalysisResponseDto>();

        if (summary is null)
        {
            throw new Exception("Failed to deserialize AI Summary response.");
        }

        return summary;
    }

    public async Task<DetailedNotesResponseDto> GenerateDetailedNotesAsync(
        DocumentAnalysisRequestDto request)
    {
        using var response = await PostAsync("/notes", request);

        await EnsureSuccessAsync(response, "Detailed Notes generation");

        var result = await response.Content
            .ReadFromJsonAsync<DetailedNotesResponseDto>();

        if (result is null)
        {
            throw new Exception("Failed to deserialize Detailed Notes.");
        }

        return result;
    }

    public async Task<ShortNotesResponseDto> GenerateShortNotesAsync(
        DocumentAnalysisRequestDto request)
    {
        using var response = await PostAsync("/shortnotes", request);

        await EnsureSuccessAsync(response, "Short Notes generation");

        var result = await response.Content
            .ReadFromJsonAsync<ShortNotesResponseDto>();

        if (result is null)
        {
            throw new Exception("Unable to deserialize Short Notes response.");
        }

        return result;
    }

    private async Task<HttpResponseMessage> PostAsync(
        string path,
        DocumentAnalysisRequestDto request)
    {
        // Always resolve the key from the authenticated user's record.
        var apiKey = await _apiKeyProvider.GetApiKeyAsync(request.UserId);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new GeminiApiKeyMissingException();
        }

        using var message = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent.Create(request)
        };

        message.Headers.TryAddWithoutValidation(GeminiApiKeyHeader, apiKey);

        var response = await _httpClient.SendAsync(message);

        // The AI service returns 401/403 when Google rejects the supplied key.
        if (response.StatusCode is HttpStatusCode.Unauthorized
            or HttpStatusCode.Forbidden)
        {
            response.Dispose();
            throw new GeminiApiKeyRejectedException();
        }

        return response;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string operation)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var error = await response.Content.ReadAsStringAsync();

        throw new Exception(
            $"{operation} failed. Status: {(int)response.StatusCode}, Response: {error}");
    }
}
