using FikaGg.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<UserGame> UserGames => Set<UserGame>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserGame>().HasKey(ug => new { ug.UserId, ug.GameId });
        builder.Entity<EventParticipant>().HasKey(p => new { p.EventId, p.UserId });

        builder.Entity<Game>().HasIndex(g => g.RawgId).IsUnique();
        
        builder.Entity<EventParticipant>()
            .Property(p => p.Status)
            .HasConversion<string>();
    }
}
