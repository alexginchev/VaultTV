using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingsController : ControllerBase
{
    private readonly AppDbContext _context;
    public RankingsController(AppDbContext context) => _context = context;

    private static RankingListDto ToDto(RankingList l) => new()
    {
        Id = l.Id,
        Title = l.Title,
        Description = l.Description,
        Criteria = l.Criteria,
        Entries = l.Entries.OrderBy(e => e.Order).Select(e => new RankingEntryDto
        {
            Id = e.Id,
            Name = e.Name,
            Score = e.Score,
            Note = e.Note,
            Order = e.Order
        }).ToList()
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RankingListDto>>> GetAll()
    {
        var lists = await _context.RankingLists.Include(l => l.Entries).ToListAsync();
        return Ok(lists.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RankingListDto>> GetById(int id)
    {
        var list = await _context.RankingLists.Include(l => l.Entries).FirstOrDefaultAsync(l => l.Id == id);
        if (list == null) return NotFound();
        return Ok(ToDto(list));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<RankingListDto>> Create(CreateRankingListDto dto)
    {
        var list = new RankingList { Title = dto.Title, Description = dto.Description, Criteria = dto.Criteria };
        _context.RankingLists.Add(list);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, ToDto(list));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var list = await _context.RankingLists.FindAsync(id);
        if (list == null) return NotFound();
        _context.RankingLists.Remove(list);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{listId}/entries")]
    public async Task<ActionResult<RankingEntryDto>> AddEntry(int listId, CreateRankingEntryDto dto)
    {
        var list = await _context.RankingLists.Include(l => l.Entries).FirstOrDefaultAsync(l => l.Id == listId);
        if (list == null) return NotFound("Ranking list not found.");

        var maxOrder = list.Entries.Any() ? list.Entries.Max(e => e.Order) : -1;

        var entry = new RankingEntry
        {
            Name = dto.Name,
            Score = dto.Score,
            Note = dto.Note,
            Order = maxOrder + 1,
            RankingListId = listId
        };

        _context.RankingEntries.Add(entry);
        await _context.SaveChangesAsync();

        return Ok(new RankingEntryDto { Id = entry.Id, Name = entry.Name, Score = entry.Score, Note = entry.Note, Order = entry.Order });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("entries/{entryId}")]
    public async Task<IActionResult> DeleteEntry(int entryId)
    {
        var entry = await _context.RankingEntries.FindAsync(entryId);
        if (entry == null) return NotFound();
        _context.RankingEntries.Remove(entry);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // The drag-and-drop endpoint: frontend sends entry ids in their new visual order
    [Authorize(Roles = "Admin")]
    [HttpPatch("{listId}/reorder")]
    public async Task<IActionResult> Reorder(int listId, ReorderDto dto)
    {
        var entries = await _context.RankingEntries
            .Where(e => e.RankingListId == listId)
            .ToListAsync();

        for (int i = 0; i < dto.OrderedEntryIds.Count; i++)
        {
            var entry = entries.FirstOrDefault(e => e.Id == dto.OrderedEntryIds[i]);
            if (entry != null) entry.Order = i;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}