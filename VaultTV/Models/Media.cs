namespace VaultTV.Models;

public class Media
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "movie";
    public string? Year { get; set; }
    public string? Rating { get; set; }
    public string? Description { get; set; }

    public int? DirectorId { get; set; }
    public Director? Director { get; set; }

    public string? PosterUrl { get; set; }
    public string? BackdropUrl { get; set; }
    public string? TrailerUrl { get; set; }

    public bool IsFeatured { get; set; } = false;
    public bool IsTrending { get; set; } = false;

    // Show-only display fields — actual season/episode data now lives in the Seasons relation
    public string? Runtime { get; set; } // movie runtime, e.g. "2h 30m"
    public string? Network { get; set; }
    public string? Status { get; set; } // e.g. "Ongoing" / "Ended"

    public ICollection<MediaCast> Cast { get; set; } = new List<MediaCast>();
    public ICollection<MediaGenre> GenreLinks { get; set; } = new List<MediaGenre>();
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}