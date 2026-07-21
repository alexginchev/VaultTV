using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaultTV.Data;
using VaultTV.DTOs;
using VaultTV.Models;

namespace VaultTV.Controllers;

[ApiController]
[Route("api/media/{mediaId}/[controller]")]
public class SeasonsController : ControllerBase
{
    private readonly AppDbContext _context;
    public SeasonsController(AppDbContext context) => _context = context;

    private static SeasonDto ToDto(Season s) => new()
    {
        Id = s.Id,
        SeasonNumber = s.SeasonNumber,
        Title = s.Title,
        Description = s.Description,
        PosterUrl = s.PosterUrl,
        Episodes = s.Episodes
            .OrderBy(e => e.EpisodeNumber)
            .Select(e => new EpisodeDto
            {
                Id = e.Id,
                EpisodeNumber = e.EpisodeNumber,
                Title = e.Title,
                Description = e.Description,
                Runtime = e.Runtime,
                AirDate = e.AirDate,
                ThumbnailUrl = e.ThumbnailUrl,
                VideoUrl = e.VideoUrl
            }).ToList()
    };

    // Public — anyone can browse a show's seasons/episodes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SeasonDto>>> GetAll(int mediaId)
    {
        var seasons = await _context.Seasons
            .Include(s => s.Episodes)
            .Where(s => s.MediaId == mediaId)
            .OrderBy(s => s.SeasonNumber)
            .ToListAsync();

        return Ok(seasons.Select(ToDto));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<SeasonDto>> CreateSeason(int mediaId, CreateSeasonDto dto)
    {
        var mediaExists = await _context.Media.AnyAsync(m => m.Id == mediaId);
        if (!mediaExists) return NotFound("Media not found.");

        var duplicate = await _context.Seasons
            .AnyAsync(s => s.MediaId == mediaId && s.SeasonNumber == dto.SeasonNumber);
        if (duplicate) return BadRequest($"Season {dto.SeasonNumber} already exists for this media.");

        var season = new Season
        {
            MediaId = mediaId,
            SeasonNumber = dto.SeasonNumber,
            Title = dto.Title,
            Description = dto.Description
        };

        _context.Seasons.Add(season);
        await _context.SaveChangesAsync();

        return Ok(ToDto(season));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{seasonId}")]
    public async Task<IActionResult> UpdateSeason(int mediaId, int seasonId, CreateSeasonDto dto)
    {
        var season = await _context.Seasons.FirstOrDefaultAsync(s => s.Id == seasonId && s.MediaId == mediaId);
        if (season == null) return NotFound();

        season.SeasonNumber = dto.SeasonNumber;
        season.Title = dto.Title;
        season.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{seasonId}")]
    public async Task<IActionResult> DeleteSeason(int mediaId, int seasonId)
    {
        var season = await _context.Seasons.FirstOrDefaultAsync(s => s.Id == seasonId && s.MediaId == mediaId);
        if (season == null) return NotFound();

        _context.Seasons.Remove(season);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Episodes — nested under a specific season
    [Authorize(Roles = "Admin")]
    [HttpPost("{seasonId}/episodes")]
    public async Task<ActionResult<EpisodeDto>> AddEpisode(int mediaId, int seasonId, CreateEpisodeDto dto)
    {
        var season = await _context.Seasons.FirstOrDefaultAsync(s => s.Id == seasonId && s.MediaId == mediaId);
        if (season == null) return NotFound("Season not found.");

        var duplicate = await _context.Episodes
            .AnyAsync(e => e.SeasonId == seasonId && e.EpisodeNumber == dto.EpisodeNumber);
        if (duplicate) return BadRequest($"Episode {dto.EpisodeNumber} already exists for this season.");

        var episode = new Episode
        {
            SeasonId = seasonId,
            EpisodeNumber = dto.EpisodeNumber,
            Title = dto.Title,
            Description = dto.Description,
            Runtime = dto.Runtime,
            AirDate = dto.AirDate,
            VideoUrl = dto.VideoUrl
        };

        _context.Episodes.Add(episode);
        await _context.SaveChangesAsync();

        return Ok(new EpisodeDto
        {
            Id = episode.Id,
            EpisodeNumber = episode.EpisodeNumber,
            Title = episode.Title,
            Description = episode.Description,
            Runtime = episode.Runtime,
            AirDate = episode.AirDate,
            ThumbnailUrl = episode.ThumbnailUrl,
            VideoUrl = episode.VideoUrl
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{seasonId}/episodes/{episodeId}")]
    public async Task<IActionResult> UpdateEpisode(int mediaId, int seasonId, int episodeId, CreateEpisodeDto dto)
    {
        var episode = await _context.Episodes
            .Include(e => e.Season)
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId && e.Season.MediaId == mediaId);
        if (episode == null) return NotFound();

        episode.EpisodeNumber = dto.EpisodeNumber;
        episode.Title = dto.Title;
        episode.Description = dto.Description;
        episode.Runtime = dto.Runtime;
        episode.AirDate = dto.AirDate;
        episode.VideoUrl = dto.VideoUrl;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{seasonId}/episodes/{episodeId}")]
    public async Task<IActionResult> DeleteEpisode(int mediaId, int seasonId, int episodeId)
    {
        var episode = await _context.Episodes
            .Include(e => e.Season)
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId && e.Season.MediaId == mediaId);
        if (episode == null) return NotFound();

        _context.Episodes.Remove(episode);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}