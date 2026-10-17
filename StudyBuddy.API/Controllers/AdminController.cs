using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.API.Authentication;
using StudyBuddy.BLL.Interfaces;

namespace StudyBuddy.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(
        AuthenticationSchemes =
            AdminApiKeyAuthenticationHandler.SchemeName + "," +
            JwtBearerDefaults.AuthenticationScheme,
        Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);

            return NoContent();
        }

        [HttpGet("users/{id:guid}/knowledge")]
        public async Task<IActionResult> GetUserKnowledge(Guid id)
        {
            var knowledge = await _userService.GetUserKnowledgeAsync(id);

            return Ok(knowledge);
        }

        [HttpGet("users/{id:guid}/notes")]
        public async Task<IActionResult> GetUserNotes(Guid id)
        {
            var notes = await _userService.GetUserNotesAsync(id);

            return Ok(notes);
        }
    }
}
