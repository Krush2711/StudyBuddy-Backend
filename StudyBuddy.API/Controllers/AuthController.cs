using Microsoft.AspNetCore.Mvc;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.BLL.DTOs.Auths;


namespace StudyBuddy.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("api/v1/auth")]
        public async Task<IActionResult> Register( [FromBody] RegisterRequestDto  request)
        {
            var responce = await _authService.RegisterAsync(request);

            return Ok(responce);
        }

    }
}
