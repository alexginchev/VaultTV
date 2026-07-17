namespace VaultTV.DTOs;

public class ActorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }
    public bool IsIncomplete { get; set; }
}

public class CreateActorDto
{
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }
}