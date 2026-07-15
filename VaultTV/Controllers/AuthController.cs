using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;
using VaultTV.Services;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("Username already taken.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already registered.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User" // public signup can NEVER create an Admin
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponseDto { Token = token, Username = user.Username, Role = user.Role });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null) return Unauthorized("Invalid username or password.");

        bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!validPassword) return Unauthorized("Invalid username or password.");

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponseDto { Token = token, Username = user.Username, Role = user.Role });
    }
}