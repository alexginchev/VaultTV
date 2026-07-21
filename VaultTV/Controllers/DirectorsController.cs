using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DirectorsController : ControllerBase
{
    private readonly AppDbContext _context;
    public DirectorsController(AppDbContext context) => _context = context;

    private static DirectorDto ToDto(Director d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Born = d.Born,
        Nationality = d.Nationality,
        Bio = d.Bio
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DirectorDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string order = "asc")
    {
        var query = _context.Directors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.Name.Contains(search));

        query = order.ToLower() == "desc"
            ? query.OrderByDescending(d => d.Name)
            : query.OrderBy(d => d.Name);

        var directors = await query.Select(d => ToDto(d)).ToListAsync();
        return Ok(directors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DirectorDto>> GetById(int id)
    {
        var director = await _context.Directors.FindAsync(id);
        if (director == null) return NotFound();
        return Ok(ToDto(director));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DirectorDto>> Create(CreateDirectorDto dto)
    {
        var director = new Director { Name = dto.Name, Born = dto.Born, Nationality = dto.Nationality, Bio = dto.Bio };
        _context.Directors.Add(director);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = director.Id }, ToDto(director));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateDirectorDto dto)
    {
        var director = await _context.Directors.FindAsync(id);
        if (director == null) return NotFound();
        director.Name = dto.Name;
        director.Born = dto.Born;
        director.Nationality = dto.Nationality;
        director.Bio = dto.Bio;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var director = await _context.Directors.FindAsync(id);
        if (director == null) return NotFound();
        _context.Directors.Remove(director);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}