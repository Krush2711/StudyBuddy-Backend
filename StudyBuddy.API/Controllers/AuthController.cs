using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.BLL.DTOs.Auths;
using StudyBuddy.BLL.DTOs.Auths.Login;
using StudyBuddy.BLL.Interfaces.Auth;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;


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

        
        [HttpPost("api/auth")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var responce = await _authService.RegisterAsync(request);

            return Ok(responce);
        }

        [HttpPost("api/login")]  
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var responce = await _authService.LoginAsync(request);
            return Ok(responce);
        }
        [HttpGet("api/login-google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {

                RedirectUri = Url.Action(nameof(GoogleResponse))
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("api/google-callback")]
        public IActionResult GoogleResponse()
        {
            return Ok("Google callback reached.");

        }

        [Authorize]
        [HttpGet("api/profile")]
        public IActionResult GetProfile()
        {
            return Ok("Authenticated");
        }
    }
}
