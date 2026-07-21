using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersManagementController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersManagementController(AppDbContext context) => _context = context;

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminUserListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string order = "asc")
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Username.Contains(search) || u.Email.Contains(search));

        query = order.ToLower() == "desc"
            ? query.OrderByDescending(u => u.Username)
            : query.OrderBy(u => u.Username);

        var users = await query
            .Select(u => new AdminUserListDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(int id, UpdateUserRoleDto dto)
    {
        if (dto.Role != "User" && dto.Role != "Admin")
            return BadRequest("Role must be either 'User' or 'Admin'.");

        // Prevent an admin from demoting themselves and getting locked out of the admin panel
        if (id == GetCurrentUserId() && dto.Role != "Admin")
            return BadRequest("You cannot change your own role away from Admin.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Role = dto.Role;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id == GetCurrentUserId())
            return BadRequest("You cannot delete your own account.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}