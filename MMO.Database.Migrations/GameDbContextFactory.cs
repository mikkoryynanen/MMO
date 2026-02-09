using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MMO.Database.Migrations;

public class GameDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
            
        // Specify MySQL version explicitly (no database connection needed)
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 40)); // Use your MySQL version
            
        // Use a dummy connection string - won't actually connect during migration creation
        optionsBuilder.UseMySql(
            "Server=localhost;Database=yourgame;User=root;Password=password;",
            serverVersion
        );
            
        return new GameDbContext(optionsBuilder.Options); 
    }
}