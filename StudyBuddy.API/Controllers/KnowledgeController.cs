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

        public KnowledgeController(IKnowledgeService knowledgeService)
        {
            _knowledgeService = knowledgeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyKnowledge()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var documents = await _knowledgeService.GetByUserAsync(userId);

            return Ok(documents);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var document = await _knowledgeService.GetByIdAsync(id);

            if (document == null)
                return NotFound();

            // Prevent users from accessing another user's document
            if (document.UserId != userId)
                return Forbid();

            return Ok(document);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var document = await _knowledgeService.GetByIdAsync(id);

            if (document == null)
                return NotFound();

            if (document.UserId != userId)
                return Forbid();

            await _knowledgeService.DeleteAsync(id);

            return NoContent();
        }
    }
}
