namespace VaultTV.Models;

public class Season
{
    public int Id { get; set; }
    public int MediaId { get; set; }
    public Media Media { get; set; } = null!;

    public int SeasonNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? PosterUrl { get; set; }

    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}