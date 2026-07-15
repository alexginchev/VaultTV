namespace VaultTV.Models;

public class RankingList
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Criteria { get; set; } // e.g. "Score", "Tier"

    public ICollection<RankingEntry> Entries { get; set; } = new List<RankingEntry>();
}