namespace VaultTV.DTOs;

public class CreateCastDto
{
    public int MediaId { get; set; }
    public string ActorName { get; set; } = string.Empty; // looked up or auto-created
    public string? Role { get; set; }
}