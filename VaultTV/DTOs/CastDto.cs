namespace VaultTV.DTOs;

public class CreateCastDto
{
    public int MediaId { get; set; }
    public int ActorId { get; set; }
    public string? Role { get; set; }
}