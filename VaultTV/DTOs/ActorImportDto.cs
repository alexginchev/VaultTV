namespace VaultTV.DTOs;

public class ActorImportItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Born { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }
}

public class ActorImportRequestDto
{
    public List<ActorImportItemDto> Actors { get; set; } = new();
}

public class ActorImportResultDto
{
    public int Imported { get; set; }
    public int Skipped { get; set; }
    public List<string> SkippedNames { get; set; } = new();
}