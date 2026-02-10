using MMO.Database.Migrations;
using MMO.Database.Shared;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// 1. Get the connection string manually (Aspire injects this)
var connectionString = builder.Configuration.GetConnectionString("accountdb");

// 2. Register the context manually so you can access the "MySqlOptions" builder
builder.Services.AddDbContext<AccountDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), 
        mysqlOptions =>
        {
            // 3. THIS is where you link the migrations assembly
            mysqlOptions.MigrationsAssembly("MMo.Database.Migrations");
            
            // Optional: Enable retry policies (common in Aspire/Cloud apps)
            mysqlOptions.EnableRetryOnFailure();
        });
});

builder.Services.AddHostedService<MigrationWorker>();

var app = builder.Build();
app.Run();