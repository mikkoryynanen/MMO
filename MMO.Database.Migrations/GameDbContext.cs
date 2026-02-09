using MMO.Database.Migrations.Models;

namespace MMO.Database.Migrations;

using Microsoft.EntityFrameworkCore;

public class GameDbContext : DbContext
{
    public DbSet<PlayerAccount> Accounts { get; set; }

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add a Unique Index for performance and security
        modelBuilder.Entity<PlayerAccount>()
            .HasIndex(a => a.AccountId)
            .IsUnique();
    }
}