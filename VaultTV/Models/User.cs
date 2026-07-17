namespace VaultTV.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? Handle { get; set; } // e.g. "@johndoe"
    public string? ProfilePictureUrl { get; set; }
    public string ThemePreference { get; set; } = "dark"; // "dark" or "light"

    public ICollection<WatchlistItem> Watchlist { get; set; } = new List<WatchlistItem>();
}