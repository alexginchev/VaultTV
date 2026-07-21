namespace VaultTV.Models;

public class Director
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }

    public ICollection<Media> Media { get; set; } = new List<Media>();
}