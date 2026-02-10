using Microsoft.EntityFrameworkCore;
using MMO.Database.Shared.Models;

namespace MMO.Database.Shared;

public class AccountDbContext : DbContext
{
    public DbSet<PlayerAccount> Accounts { get; set; }
    public DbSet<SessionToken> SessionTokens { get; set; }

    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add a Unique Index for performance and security
        modelBuilder.Entity<PlayerAccount>()
            .HasIndex(a => a.AccountId)
            .IsUnique();
        
        modelBuilder.Entity<PlayerAccount>()
            .HasIndex(a => a.Username)
            .IsUnique();
        
        modelBuilder.Entity<SessionToken>()
            .HasIndex(t => t.AccountId)
            .IsUnique();
    }
}