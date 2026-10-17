using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Knowledge.Interfaces;
using System.Security.Claims;

namespace StudyBuddy.API.Controllers
{
    [ApiController]
    [Route("api/knowledge")]
    [Authorize]
    public class KnowledgeController : Controller
    {
        private readonly IKnowledgeService _knowledgeService;
        private readonly IDetailedNotesService _detailedNotesService;
        private readonly IShortNotesService _shortNotesService;

        public KnowledgeController(
            IKnowledgeService knowledgeService,
            IDetailedNotesService detailedNotesService,
            IShortNotesService shortNotesService)
        {
            _knowledgeService = knowledgeService;
            _detailedNotesService = detailedNotesService;
            _shortNotesService = shortNotesService;
        }

        // GET /api/knowledge?timeline=today|30days|all&subject=Math
        [HttpGet]
        public async Task<IActionResult> GetMyKnowledge(
            [FromQuery] string? timeline = null,
            [FromQuery] string? subject = null,
            [FromQuery] string? tag = null)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            // Preserve existing behaviour: no timeline means all documents.
            if (!TryResolveTimeline(timeline, defaultTo30Days: false, out var fromUtc))
                return BadRequest(new { message = "Invalid timeline. Use today, 30days, or all." });

            var documents = await _knowledgeService.GetByUserAsync(
                userId,
                fromUtc,
                NormalizeSubjectTag(subject, tag));

            return Ok(documents);
        }

        // GET /api/knowledge/notes?timeline=today|30days|all&subject=Math
        [HttpGet("notes")]
        public async Task<IActionResult> GetMyNotes(
            [FromQuery] string? timeline = null,
            [FromQuery] string? subject = null,
            [FromQuery] string? tag = null)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            // Default for the notes endpoint is the last 30 days.
            if (!TryResolveTimeline(timeline, defaultTo30Days: true, out var fromUtc))
                return BadRequest(new { message = "Invalid timeline. Use today, 30days, or all." });

            var resolvedTimeline = string.IsNullOrWhiteSpace(timeline)
                ? "30days"
                : timeline.Trim().ToLowerInvariant();

            var subjectTag = NormalizeSubjectTag(subject, tag);

            var detailedNotes = await _detailedNotesService.GetByUserAsync(
                userId, fromUtc, subjectTag);
            var shortNotes = await _shortNotesService.GetByUserAsync(
                userId, fromUtc, subjectTag);

            return Ok(new
            {
                userId,
                timeline = resolvedTimeline,
                subject = subjectTag,
                detailedNotes,
                shortNotes
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var document = await _knowledgeService.GetByIdAsync(id);

            if (document == null)
                return NotFound();

            // Ownership check: users may only read their own documents.
            if (document.UserId != userId)
                return Forbid();

            return Ok(document);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var document = await _knowledgeService.GetByIdAsync(id);

            if (document == null)
                return NotFound();

            // Ownership check: users may only delete their own documents.
            if (document.UserId != userId)
                return Forbid();

            await _knowledgeService.DeleteAsync(id);

            return NoContent();
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;

            // Never accept a user id from the client; always use the JWT claim.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return !string.IsNullOrWhiteSpace(userIdClaim) &&
                   Guid.TryParse(userIdClaim, out userId);
        }

        private static string? NormalizeSubjectTag(string? subject, string? tag)
        {
            var value = string.IsNullOrWhiteSpace(subject) ? tag : subject;

            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool TryResolveTimeline(
            string? timeline,
            bool defaultTo30Days,
            out DateTime? fromUtc)
        {
            fromUtc = null;

            var value = string.IsNullOrWhiteSpace(timeline)
                ? (defaultTo30Days ? "30days" : "all")
                : timeline.Trim().ToLowerInvariant();

            switch (value)
            {
                case "today":
                    // CreatedAt is stored using DateTime.UtcNow, so use UTC boundaries.
                    fromUtc = DateTime.UtcNow.Date;
                    return true;

                case "30days":
                    fromUtc = DateTime.UtcNow.AddDays(-30);
                    return true;

                case "all":
                    fromUtc = null;
                    return true;

                default:
                    return false;
            }
        }
    }
}
