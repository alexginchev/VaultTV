namespace VaultTV.Models;

public class Media
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "movie"; // "movie" or "show"
    public string? Year { get; set; }
    public string? Rating { get; set; }
    public string? Description { get; set; }
    public string? Director { get; set; }
    public string? Genres { get; set; } // stored as comma-separated string, split in the DTO layer

    public string? PosterUrl { get; set; }
    public string? BackdropUrl { get; set; }

    // Show-only fields
    public string? Seasons { get; set; }
    public string? Runtime { get; set; }
    public string? Network { get; set; }
    public string? Status { get; set; }

    public ICollection<MediaCast> Cast { get; set; } = new List<MediaCast>();
}