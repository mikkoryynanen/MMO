using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MMO.Database.Shared;

namespace MMO.Database.Migrations;

public class GameDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
            
        // Specify MySQL version explicitly (no database connection needed)
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 40)); // Use your MySQL version
            
        // Use a dummy connection string - won't actually connect during migration creation
        optionsBuilder.UseMySql(
            "Server=localhost;Database=yourgame;User=root;Password=password;",
            serverVersion,
            mysqlOptions => mysqlOptions.MigrationsAssembly("MMO.Database.Migrations")
        );
            
        return new AccountDbContext(optionsBuilder.Options); 
    }
}