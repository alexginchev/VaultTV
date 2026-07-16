using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // every endpoint here requires a logged-in user, admin or not
public class WatchlistController : ControllerBase
{
    private readonly AppDbContext _context;
    public WatchlistController(AppDbContext context) => _context = context;

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WatchlistItemDto>>> GetMyWatchlist()
    {
        var userId = GetCurrentUserId();

        var items = await _context.WatchlistItems
            .Include(w => w.Media)
            .Where(w => w.UserId == userId)
            .Select(w => new WatchlistItemDto
            {
                Id = w.Id,
                AddedAt = w.AddedAt,
                MediaId = w.MediaId,
                MediaTitle = w.Media.Title,
                PosterUrl = w.Media.PosterUrl
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<WatchlistItemDto>> AddToWatchlist(CreateWatchlistItemDto dto)
    {
        var userId = GetCurrentUserId();

        var media = await _context.Media.FindAsync(dto.MediaId);
        if (media == null) return BadRequest("Media not found.");

        var alreadyAdded = await _context.WatchlistItems
            .AnyAsync(w => w.UserId == userId && w.MediaId == dto.MediaId);
        if (alreadyAdded) return BadRequest("Already in your watchlist.");

        var item = new WatchlistItem { UserId = userId, MediaId = dto.MediaId };
        _context.WatchlistItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new WatchlistItemDto
        {
            Id = item.Id,
            AddedAt = item.AddedAt,
            MediaId = media.Id,
            MediaTitle = media.Title,
            PosterUrl = media.PosterUrl
        });
    }

    [HttpDelete("{mediaId}")]
    public async Task<IActionResult> RemoveFromWatchlist(int mediaId)
    {
        var userId = GetCurrentUserId();

        var item = await _context.WatchlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.MediaId == mediaId);
        if (item == null) return NotFound();

        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}