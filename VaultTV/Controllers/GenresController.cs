using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly AppDbContext _context;
    public GenresController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
    {
        var genres = await _context.Genres
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto { Id = g.Id, Name = g.Name })
            .ToListAsync();
        return Ok(genres);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GenreDto>> Create(CreateGenreDto dto)
    {
        var exists = await _context.Genres.AnyAsync(g => g.Name.ToLower() == dto.Name.ToLower());
        if (exists) return BadRequest("Genre already exists.");

        var genre = new Genre { Name = dto.Name.Trim() };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return Ok(new GenreDto { Id = genre.Id, Name = genre.Name });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateGenreDto dto)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) return NotFound();
        genre.Name = dto.Name.Trim();
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) return NotFound();
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}