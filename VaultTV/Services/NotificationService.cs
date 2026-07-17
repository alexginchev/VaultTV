using VaultTV.Data;
using VaultTV.Models;

namespace VaultTV.Services;

public class NotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task NotifyAsync(string message, string type = "Info", int? relatedEntityId = null)
    {
        var notification = new Notification
        {
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}