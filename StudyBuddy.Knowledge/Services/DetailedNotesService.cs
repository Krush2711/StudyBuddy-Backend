using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.Interfaces;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Services;

public class DetailedNotesService : IDetailedNotesService
{
    private readonly IAiService _aiService;
    private readonly IDetailedNotesRepository _repository;

    public DetailedNotesService(
        IAiService aiService,
        IDetailedNotesRepository repository)
    {
        _aiService = aiService;
        _repository = repository;
    }

    public async Task GenerateAsync(DocumentAnalysisRequestDto request)
    {
        // Call FastAPI
        var response = await _aiService.GenerateDetailedNotesAsync(request);

        // Convert DTO -> Mongo Document
        var document = new DetailedNotesDocument

        {
            Id = response.Id,
            UserId = response.UserId,
            RawDocumentId = response.RawDocumentId,
            FileName = response.FileName,

            Title = response.Title,
            Overview = response.Overview,

            ImportantPoints = response.ImportantPoints,
            Examples = response.Examples,
            BestPractices = response.BestPractices,
            CommonMistakes = response.CommonMistakes,
            RevisionTips = response.RevisionTips,

            CreatedAt = DateTime.UtcNow,

            Sections = response.Sections
                .Select(x => new NoteSection
                {
                    Heading = x.Heading,
                    Content = x.Content
                })
                .ToList()
        };

        // Save into MongoDB
        await _repository.CreateAsync(document);
    }
}