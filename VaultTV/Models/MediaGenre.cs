namespace VaultTV.Models;

public class MediaGenre
{
    public int MediaId { get; set; }
    public Media Media { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}