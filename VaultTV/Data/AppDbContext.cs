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
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MediaGenre> MediaGenres => Set<MediaGenre>();
    public DbSet<Director> Directors => Set<Director>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaGenre>().HasKey(mg => new { mg.MediaId, mg.GenreId });

        modelBuilder.Entity<MediaGenre>()
            .HasOne(mg => mg.Media)
            .WithMany(m => m.GenreLinks)
            .HasForeignKey(mg => mg.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MediaGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(g => g.MediaLinks)
            .HasForeignKey(mg => mg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();

        // A movie/show can have a director; deleting a director should NOT delete their media —
        // just null out the reference, so historical media entries aren't destroyed.
        modelBuilder.Entity<Media>()
            .HasOne(m => m.Director)
            .WithMany(d => d.Media)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Director>()
            .HasIndex(d => d.Name)
            .IsUnique();

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

        // A media can't have two seasons with the same number
        modelBuilder.Entity<Season>()
            .HasIndex(s => new { s.MediaId, s.SeasonNumber })
            .IsUnique();

        modelBuilder.Entity<Season>()
            .HasOne(s => s.Media)
            .WithMany(m => m.Seasons)
            .HasForeignKey(s => s.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        // A season can't have two episodes with the same number
        modelBuilder.Entity<Episode>()
            .HasIndex(e => new { e.SeasonId, e.EpisodeNumber })
            .IsUnique();

        modelBuilder.Entity<Episode>()
            .HasOne(e => e.Season)
            .WithMany(s => s.Episodes)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}