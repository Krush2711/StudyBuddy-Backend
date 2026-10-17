using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.BLL.DTOs.User;
using StudyBuddy.BLL.Interfaces;
using StudyBuddy.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var profile = await _userService.GetProfileAsync(userId);

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequestDto request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var updatedProfile = await _userService.UpdateProfileAsync(userId, request);

        return Ok(updatedProfile);
    }

    /// <summary>
    /// Saves (or replaces) the authenticated user's Gemini API key.
    /// The key is encrypted at rest and is never echoed back.
    /// </summary>
    [HttpPost("me/gemini-key")]
    public async Task<IActionResult> SaveGeminiApiKey(
        [FromBody] SetGeminiApiKeyRequestDto request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _userService.SaveGeminiApiKeyAsync(userId, request);

        return Ok(new
        {
            message = "Gemini API key saved successfully."
        });
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;

        // Never accept a user id from the client; always use the JWT claim.
        var userIdClaim = User
            .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return !string.IsNullOrWhiteSpace(userIdClaim)
               && Guid.TryParse(userIdClaim, out userId);
    }
}