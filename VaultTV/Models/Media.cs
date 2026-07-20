using VaultTV.Models;

namespace VaultTV.DTOs;

public class MediaDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Year { get; set; }
    public string? Rating { get; set; }
    public string? Description { get; set; }
    public string? Director { get; set; }
    public ICollection<MediaGenre> GenreLinks { get; set; } = new List<MediaGenre>();
    public string? PosterUrl { get; set; }
    public string? BackdropUrl { get; set; }
    public string? Seasons { get; set; }
    public string? Runtime { get; set; }
    public string? Network { get; set; }
    public string? Status { get; set; }
    public List<CastMemberDto> Cast { get; set; } = new();
}

public class CastMemberDto
{
    public int ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string? Role { get; set; }
}

public class CreateMediaDto
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "movie";
    public string? Year { get; set; }
    public string? Rating { get; set; }
    public string? Description { get; set; }
    public string? Director { get; set; }
    public List<string> GenreNames { get; set; } = new(); // names, not IDs — smart create/match like actors
    public string? Seasons { get; set; }
    public string? Runtime { get; set; }
    public string? Network { get; set; }
    public string? Status { get; set; }
}