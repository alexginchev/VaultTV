namespace VaultTV.DTOs;

public class SetTopRankDto
{
    public int ActorId { get; set; }
    public int Rank { get; set; } // 1-10
}

public class ClearTopRankDto
{
    public int ActorId { get; set; }
}