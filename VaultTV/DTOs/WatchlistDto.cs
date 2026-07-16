namespace VaultTV.DTOs;

public class WatchlistItemDto
{
    public int Id { get; set; }
    public DateTime AddedAt { get; set; }
    public int MediaId { get; set; }
    public string MediaTitle { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
}

public class CreateWatchlistItemDto
{
    public int MediaId { get; set; }
}