using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Services;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any logged-in user, admin or not
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;

    public UsersController(AppDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static UserProfileDto ToDto(Models.User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        Role = u.Role,
        Handle = u.Handle,
        ProfilePictureUrl = u.ProfilePictureUrl,
        ThemePreference = u.ThemePreference,
        CreatedAt = u.CreatedAt
    };

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return NotFound();
        return Ok(ToDto(user));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMe(UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Handle))
        {
            var handleTaken = await _context.Users
                .AnyAsync(u => u.Handle == dto.Handle && u.Id != user.Id);
            if (handleTaken) return BadRequest("That handle is already taken.");
        }

        user.Handle = dto.Handle;
        user.ThemePreference = dto.ThemePreference == "light" ? "light" : "dark";

        await _context.SaveChangesAsync();
        return Ok(ToDto(user));
    }

    [HttpPost("me/avatar")]
    public async Task<ActionResult<UserProfileDto>> UploadAvatar(IFormFile file)
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return NotFound();

        _fileService.DeleteImage(user.ProfilePictureUrl);
        user.ProfilePictureUrl = await _fileService.SaveImageAsync(file, "avatars");

        await _context.SaveChangesAsync();
        return Ok(ToDto(user));
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return NotFound();

        bool validCurrent = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
        if (!validCurrent) return BadRequest("Current password is incorrect.");

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return BadRequest("New password must be at least 6 characters.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}