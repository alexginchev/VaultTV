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
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetAll()
    {
        var actors = await _context.Actors
            .Select(a => new ActorDto { Id = a.Id, Name = a.Name, Born = a.Born, Nationality = a.Nationality, Bio = a.Bio })
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
}