namespace VaultTV.DTOs;

public class EpisodeDto
{
    public int Id { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Runtime { get; set; }
    public string? AirDate { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoUrl { get; set; }
}

public class CreateEpisodeDto
{
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Runtime { get; set; }
    public string? AirDate { get; set; }
    public string? VideoUrl { get; set; }
}