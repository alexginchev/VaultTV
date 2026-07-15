namespace VaultTV.Models;

public class WatchlistItem
{
    public int Id { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int MediaId { get; set; }
    public Media Media { get; set; } = null!;
}