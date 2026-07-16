namespace VaultTV.DTOs;

public class RankingListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Criteria { get; set; }
    public List<RankingEntryDto> Entries { get; set; } = new();
}

public class CreateRankingListDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Criteria { get; set; }
}

public class RankingEntryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; }
}

public class CreateRankingEntryDto
{
    public string Name { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string? Note { get; set; }
}

public class ReorderDto
{
    public List<int> OrderedEntryIds { get; set; } = new(); // entry ids in their new visual order
}