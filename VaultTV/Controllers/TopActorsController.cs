using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopActorsController : ControllerBase
{
    private readonly AppDbContext _context;
    public TopActorsController(AppDbContext context) => _context = context;

    // Public — anyone can see the Top 10
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetTop10()
    {
        var actors = await _context.Actors
            .Where(a => a.TopRank != null)
            .OrderBy(a => a.TopRank)
            .Select(a => new ActorDto
            {
                Id = a.Id,
                Name = a.Name,
                Born = a.Born,
                Nationality = a.Nationality,
                Bio = a.Bio,
                IsIncomplete = a.IsIncomplete,
                TopRank = a.TopRank
            })
            .ToListAsync();

        return Ok(actors);
    }

    // Admin sets a specific actor into a specific rank slot (1-10)
    [Authorize(Roles = "Admin")]
    [HttpPut("rank")]
    public async Task<IActionResult> SetRank(SetTopRankDto dto)
    {
        if (dto.Rank < 1 || dto.Rank > 10)
            return BadRequest("Rank must be between 1 and 10.");

        var actor = await _context.Actors.FindAsync(dto.ActorId);
        if (actor == null) return NotFound("Actor not found.");

        // If another actor already holds this rank, bump them out (set to null)
        var currentHolder = await _context.Actors
            .FirstOrDefaultAsync(a => a.TopRank == dto.Rank && a.Id != dto.ActorId);
        if (currentHolder != null) currentHolder.TopRank = null;

        actor.TopRank = dto.Rank;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Admin removes an actor from the Top 10 entirely
    [Authorize(Roles = "Admin")]
    [HttpPut("clear")]
    public async Task<IActionResult> ClearRank(ClearTopRankDto dto)
    {
        var actor = await _context.Actors.FindAsync(dto.ActorId);
        if (actor == null) return NotFound("Actor not found.");

        actor.TopRank = null;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}