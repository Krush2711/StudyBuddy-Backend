using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.API.Requests.RawDocument;
using StudyBuddy.BLL.DTOs.RawDocument;
using StudyBuddy.BLL.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudyBuddy.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/ingestion")]
    public class IngestionController : ControllerBase
    {
        private readonly IRawDocumentService _rawDocumentService;

        public IngestionController(IRawDocumentService rawDocumentService)
        {
            _rawDocumentService = rawDocumentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] UploadRawDocumentRequest request)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId) ||
                !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized();
            }


            var dto = new UploadRawDocumentDto
            {
                FileStream = request.File.OpenReadStream(),
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,

                SourceType = request.SourceType,
                Title = request.Title,
                Description = request.Description,
                Subject = string.IsNullOrWhiteSpace(request.Subject)
                    ? request.Tag
                    : request.Subject,
                LearningDate = request.LearningDate
            };

            var response = await _rawDocumentService.UploadAsync(
                parsedUserId,
                dto);

            return Ok(response);
        }

    }
}