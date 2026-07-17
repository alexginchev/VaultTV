using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActorsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ActorsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetAll(
    [FromQuery] string? search,
    [FromQuery] string sortBy = "name",
    [FromQuery] string order = "asc",
    [FromQuery] bool? incompleteOnly = null)
    {
        var query = _context.Actors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Name.Contains(search));

        if (incompleteOnly == true)
            query = query.Where(a => a.IsIncomplete);

        query = order.ToLower() == "desc"
            ? query.OrderByDescending(a => a.Name)
            : query.OrderBy(a => a.Name);

        var actors = await query
            .Select(a => new ActorDto
            {
                Id = a.Id,
                Name = a.Name,
                Born = a.Born,
                Nationality = a.Nationality,
                Bio = a.Bio,
                IsIncomplete = a.IsIncomplete
            })
            .ToListAsync();

        return Ok(actors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ActorDto>> GetById(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return NotFound();
        return Ok(new ActorDto { Id = actor.Id, Name = actor.Name, Born = actor.Born, Nationality = actor.Nationality, Bio = actor.Bio });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ActorDto>> Create(CreateActorDto dto)
    {
        var actor = new Actor { Name = dto.Name, Born = dto.Born, Nationality = dto.Nationality, Bio = dto.Bio };
        _context.Actors.Add(actor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = actor.Id },
            new ActorDto { Id = actor.Id, Name = actor.Name, Born = actor.Born, Nationality = actor.Nationality, Bio = actor.Bio });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateActorDto dto)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return NotFound();
        actor.Name = dto.Name;
        actor.Born = dto.Born;
        actor.Nationality = dto.Nationality;
        actor.Bio = dto.Bio;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return NotFound();
        _context.Actors.Remove(actor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("import")]
    public async Task<ActionResult<ActorImportResultDto>> Import(ActorImportRequestDto dto)
    {
        var result = new ActorImportResultDto();

        // Load existing names once, lowercase, to check duplicates in-memory instead of one query per row
        var existingNames = await _context.Actors
            .Select(a => a.Name.ToLower())
            .ToListAsync();

        var existingSet = new HashSet<string>(existingNames);
        var newActors = new List<Actor>();

        foreach (var item in dto.Actors)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                result.Skipped++;
                continue;
            }

            var normalizedName = item.Name.Trim().ToLower();

            if (existingSet.Contains(normalizedName))
            {
                result.Skipped++;
                result.SkippedNames.Add(item.Name);
                continue;
            }

            newActors.Add(new Actor
            {
                Name = item.Name.Trim(),
                Born = item.Born,
                Nationality = item.Nationality,
                Bio = item.Bio,
                IsIncomplete = false // came from a real data source, not auto-created
            });

            existingSet.Add(normalizedName); // guards against duplicates *within* the same import batch too
        }

        _context.Actors.AddRange(newActors);
        await _context.SaveChangesAsync();

        result.Imported = newActors.Count;
        return Ok(result);
    }
}