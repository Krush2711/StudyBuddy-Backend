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
        // claim -> sub / find frist / sub / -> null-> unautarize -> guid -> id -> get profile -> profile.
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim);
        var profile = await _userService.GetProfileAsync(userId);

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequestDto request)
    {
        // Read userId from JWT
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        // Call service
        var userId = Guid.Parse(userIdClaim);
        var updatedProfile = await _userService.UpdateProfileAsync(userId, request);

        // Return Ok(updatedProfile)
        return Ok(updatedProfile);
    }
}