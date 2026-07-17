namespace VaultTV.Models;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // e.g. "IncompleteActor", "Info", "Warning"
    public int? RelatedEntityId { get; set; } // e.g. the Actor's Id, so the frontend can link to it
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}