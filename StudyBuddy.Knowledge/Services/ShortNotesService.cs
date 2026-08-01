using StudyBuddy.Ai.DTOs;
using StudyBuddy.Ai.Interfaces;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Services;

public class ShortNotesService : IShortNotesService
{
    private readonly IAiService _aiService;
    private readonly IShortNotesRepository _repository;

    public ShortNotesService(
        IAiService aiService,
        IShortNotesRepository repository)
    {
        _aiService = aiService;
        _repository = repository;
    }

    public async Task GenerateAsync(
        DocumentAnalysisRequestDto request)
    {
        // Call FastAPI
        var response =
            await _aiService.GenerateShortNotesAsync(request);

        // Convert DTO -> Mongo Document
        var document = new ShortNotesDocument
        {
            Id = response.Id,
            UserId = response.UserId,
            RawDocumentId = response.RawDocumentId,
            FileName = response.FileName,

            Title = response.Title,
            Summary = response.Summary,

            KeyConcepts = response.KeyConcepts,
            ImportantDefinitions = response.ImportantDefinitions,
            Formulas = response.Formulas,
            Syntax = response.Syntax,
            ImportantPoints = response.ImportantPoints,
            InterviewTips = response.InterviewTips,
            RevisionChecklist = response.RevisionChecklist,
            Mnemonics = response.Mnemonics,

            CreatedAt = DateTime.UtcNow,

            Sections = response.Sections
                .Select(x => new NoteSection
                {
                    Heading = x.Heading,
                    Content = x.Content
                })
                .ToList()
        };

        await _repository.CreateAsync(document);
    }
}