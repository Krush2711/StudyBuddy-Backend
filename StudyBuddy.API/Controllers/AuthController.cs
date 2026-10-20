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

        
        [HttpPost("/api/auth")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var responce = await _authService.RegisterAsync(request);

            return Ok(responce);
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var responce = await _authService.LoginAsync(request);
            return Ok(responce);
        }

        /// <summary>
        /// Exchanges a refresh token for a new access/refresh pair. No [Authorize]:
        /// the caller's access token has usually already expired by design.
        /// </summary>
        [HttpPost("/api/refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDto request)
        {
            var response = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(response);
        }

        /// <summary>Revokes a refresh token. Always 204, even for unknown values.</summary>
        [HttpPost("/api/logout")]
        public async Task<IActionResult> Logout(RefreshRequestDto request)
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return NoContent();
        }
        [HttpGet("/api/login-google")]
        public IActionResult GoogleLogin()
        {
            // Build the post-login redirect explicitly from the authorized request
            // (ForwardedHeaders maps X-Forwarded-Proto / X-Forwarded-Host to the
            // real public scheme/host). Url.Action() alone produces a relative
            // path that resolves incorrectly behind Blitz Cloud's reverse proxy.
            var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google-response";

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri
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

        [Authorize]
        [HttpGet("/api/profile")]
        public IActionResult GetProfile()
        {
            return Ok("Authenticated");
        }
    }
}
