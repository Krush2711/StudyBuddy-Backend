using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.BLL.DTOs.Auths;
using StudyBuddy.BLL.DTOs.Auths.Login;
using StudyBuddy.BLL.Interfaces.Auth;
using System.Security.Claims;



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

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return BadRequest("Google authentication failed.");
            }

            var claims = result.Principal!.Claims;

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
;            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email)  || string.IsNullOrEmpty(name))
            {
                return BadRequest("Required Google claims are missing.");
            }
            var response = await _authService.GoogleLoginAsync(googleId, email, name);

            return Ok(response);


        }

        //[Authorize]
        //[HttpGet("api/profile")]
        //public IActionResult GetProfile()
        //{
        //    return Ok("Authenticated");
        //}
    }
}
