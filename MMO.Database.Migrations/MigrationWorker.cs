using Microsoft.EntityFrameworkCore;
using MMO.Database.Migrations.Models;

namespace MMO.Database.Migrations;

public class MigrationWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Create a scope to retrieve the DbContext
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            
            // This ensures the DB is created and migrations are applied
            await dbContext.Database.MigrateAsync(stoppingToken);
            Console.WriteLine("✅ Database migration completed successfully.");
            
            await dbContext.Database.EnsureCreatedAsync(stoppingToken);
        
            // Seed Data
            // var strategy = dbContext.Database.CreateExecutionStrategy();
            // await strategy.ExecuteAsync(async () =>
            // {
            //     if (!await dbContext.Accounts.AnyAsync(stoppingToken))
            //     {
            //         dbContext.Accounts.AddRange(
            //             new PlayerAccount() { AccountId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }
            //         );
            //         await dbContext.SaveChangesAsync(stoppingToken);
            //     }
            // });
            // Console.WriteLine("Seeded data to db");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database migration failed: {ex.Message}");
            // Optional: exit with error code so Aspire shows it failed
            Environment.Exit(1);
        }

        // Optional: Stop the application once migration is done
        // If you leave this running, the dashboard shows it as "Running" (Green)
        // If you stop it, Aspire might try to restart it depending on configuration.
        // For now, keeping it alive is often cleaner in the Dashboard.
    }
}