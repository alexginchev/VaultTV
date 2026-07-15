namespace VaultTV.Models;

public class MediaCast
{
    public int Id { get; set; }
    public string? Role { get; set; }

    public int MediaId { get; set; }
    public Media Media { get; set; } = null!;

    public int ActorId { get; set; }
    public Actor Actor { get; set; } = null!;
}