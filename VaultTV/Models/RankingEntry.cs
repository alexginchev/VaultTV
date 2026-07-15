namespace VaultTV.Models;

public class RankingEntry
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; } // controls drag-and-drop position

    public int RankingListId { get; set; }
    public RankingList RankingList { get; set; } = null!;
}