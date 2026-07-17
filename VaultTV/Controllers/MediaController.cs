using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;
using VaultTV.Services;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;
    private readonly NotificationService _notificationService;

    public MediaController(AppDbContext context, FileService fileService, NotificationService notificationService)
    {
        _context = context;
        _fileService = fileService;
        _notificationService = notificationService;
    }

    private static MediaDto ToDto(Media m) => new()
    {
        Id = m.Id,
        Title = m.Title,
        Type = m.Type,
        Year = m.Year,
        Rating = m.Rating,
        Description = m.Description,
        Director = m.Director,
        Genres = string.IsNullOrEmpty(m.Genres)
            ? new List<string>()
            : m.Genres.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
        PosterUrl = m.PosterUrl,
        BackdropUrl = m.BackdropUrl,
        Seasons = m.Seasons,
        Runtime = m.Runtime,
        Network = m.Network,
        Status = m.Status,
        Cast = m.Cast.Select(c => new CastMemberDto
        {
            ActorId = c.ActorId,
            ActorName = c.Actor.Name,
            Role = c.Role
        }).ToList()
    };

    // Public — anyone can browse
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaDto>>> GetAll()
    {
        var media = await _context.Media.Include(m => m.Cast).ThenInclude(c => c.Actor).ToListAsync();
        return Ok(media.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaDto>> GetById(int id)
    {
        var media = await _context.Media.Include(m => m.Cast).ThenInclude(c => c.Actor)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (media == null) return NotFound();
        return Ok(ToDto(media));
    }

    // Admin-only — create/edit/delete
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MediaDto>> Create(CreateMediaDto dto)
    {
        var media = new Media
        {
            Title = dto.Title,
            Type = dto.Type,
            Year = dto.Year,
            Rating = dto.Rating,
            Description = dto.Description,
            Director = dto.Director,
            Genres = string.Join(",", dto.Genres),
            Seasons = dto.Seasons,
            Runtime = dto.Runtime,
            Network = dto.Network,
            Status = dto.Status
        };

        _context.Media.Add(media);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = media.Id }, ToDto(media));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateMediaDto dto)
    {
        var media = await _context.Media.FindAsync(id);
        if (media == null) return NotFound();

        media.Title = dto.Title;
        media.Type = dto.Type;
        media.Year = dto.Year;
        media.Rating = dto.Rating;
        media.Description = dto.Description;
        media.Director = dto.Director;
        media.Genres = string.Join(",", dto.Genres);
        media.Seasons = dto.Seasons;
        media.Runtime = dto.Runtime;
        media.Network = dto.Network;
        media.Status = dto.Status;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var media = await _context.Media.FindAsync(id);
        if (media == null) return NotFound();

        _fileService.DeleteImage(media.PosterUrl);
        _fileService.DeleteImage(media.BackdropUrl);

        _context.Media.Remove(media);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Image uploads — admin only
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/poster")]
    public async Task<ActionResult<MediaDto>> UploadPoster(int id, IFormFile file)
    {
        var media = await _context.Media.Include(m => m.Cast).ThenInclude(c => c.Actor)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (media == null) return NotFound();

        _fileService.DeleteImage(media.PosterUrl); // replace old file if one existed
        media.PosterUrl = await _fileService.SaveImageAsync(file, "posters");

        await _context.SaveChangesAsync();
        return Ok(ToDto(media));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/backdrop")]
    public async Task<ActionResult<MediaDto>> UploadBackdrop(int id, IFormFile file)
    {
        var media = await _context.Media.Include(m => m.Cast).ThenInclude(c => c.Actor)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (media == null) return NotFound();

        _fileService.DeleteImage(media.BackdropUrl);
        media.BackdropUrl = await _fileService.SaveImageAsync(file, "backdrops");

        await _context.SaveChangesAsync();
        return Ok(ToDto(media));
    }

    // Cast management — admin only
    [Authorize(Roles = "Admin")]
    [HttpPost("cast")]
    public async Task<IActionResult> AddCastMember(CreateCastDto dto)
    {
        var mediaExists = await _context.Media.AnyAsync(m => m.Id == dto.MediaId);
        if (!mediaExists) return BadRequest("Media not found.");

        if (string.IsNullOrWhiteSpace(dto.ActorName))
            return BadRequest("Actor name is required.");

        // Case-insensitive lookup, so "tom hanks" matches an existing "Tom Hanks"
        var actor = await _context.Actors
            .FirstOrDefaultAsync(a => a.Name.ToLower() == dto.ActorName.ToLower());

        bool wasAutoCreated = false;

        if (actor == null)
        {
            actor = new Actor { Name = dto.ActorName, IsIncomplete = true };
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync(); // save now so actor.Id is populated below
            wasAutoCreated = true;
        }

        var alreadyCast = await _context.MediaCasts
            .AnyAsync(c => c.MediaId == dto.MediaId && c.ActorId == actor.Id);
        if (alreadyCast) return BadRequest("This actor is already listed for this media.");

        var cast = new MediaCast { MediaId = dto.MediaId, ActorId = actor.Id, Role = dto.Role };
        _context.MediaCasts.Add(cast);
        await _context.SaveChangesAsync();

        if (wasAutoCreated)
        {
            await _notificationService.NotifyAsync(
                $"Actor \"{actor.Name}\" was auto-created and needs additional details.",
                "IncompleteActor",
                actor.Id
            );
        }

        return Ok();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("cast/{castId}")]
    public async Task<IActionResult> RemoveCastMember(int castId)
    {
        var cast = await _context.MediaCasts.FindAsync(castId);

        if (cast == null)
            return NotFound();

        _context.MediaCasts.Remove(cast);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}