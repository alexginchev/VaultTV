using VaultTV.Models;
using Microsoft.EntityFrameworkCore;

namespace VaultTV.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Media> Media => Set<Media>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<MediaCast> MediaCasts => Set<MediaCast>();
    public DbSet<RankingList> RankingLists => Set<RankingList>();
    public DbSet<RankingEntry> RankingEntries => Set<RankingEntry>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();

    public DbSet<Notification> Notifications => Set<Notification>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaCast>()
            .HasIndex(mc => new { mc.MediaId, mc.ActorId })
            .IsUnique();

        modelBuilder.Entity<MediaCast>()
            .HasOne(mc => mc.Media)
            .WithMany(m => m.Cast)
            .HasForeignKey(mc => mc.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MediaCast>()
            .HasOne(mc => mc.Actor)
            .WithMany(a => a.Appearances)
            .HasForeignKey(mc => mc.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RankingEntry>()
            .HasOne(e => e.RankingList)
            .WithMany(l => l.Entries)
            .HasForeignKey(e => e.RankingListId)
            .OnDelete(DeleteBehavior.Cascade);

        // A user can't add the same media to their watchlist twice
        modelBuilder.Entity<WatchlistItem>()
            .HasIndex(w => new { w.UserId, w.MediaId })
            .IsUnique();

        modelBuilder.Entity<WatchlistItem>()
            .HasOne(w => w.User)
            .WithMany(u => u.Watchlist)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WatchlistItem>()
            .HasOne(w => w.Media)
            .WithMany()
            .HasForeignKey(w => w.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure usernames and emails are unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}