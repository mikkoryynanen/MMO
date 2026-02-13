using Microsoft.EntityFrameworkCore;
using MMO.WorldServer.Database.Models;

namespace MMO.WorldServer.Database;

public class WorldDbContext : DbContext
{
    public DbSet<WorldEntity> WorldEntities { get; set; }

    public WorldDbContext(DbContextOptions<WorldDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add a Unique Index for performance and security
        modelBuilder.Entity<WorldEntity>()
            .HasIndex(e => e.Id)
            .IsUnique();
    }
}