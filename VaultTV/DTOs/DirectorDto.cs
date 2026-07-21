namespace VaultTV.DTOs;

public class DirectorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }
}

public class CreateDirectorDto
{
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }
}