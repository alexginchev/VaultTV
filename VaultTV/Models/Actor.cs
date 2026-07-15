namespace VaultTV.Models;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }

    public ICollection<MediaCast> Appearances { get; set; } = new List<MediaCast>();
}