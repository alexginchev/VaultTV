namespace VaultTV.Models;

public class Episode
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Runtime { get; set; }
    public string? AirDate { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoUrl { get; set; }
}