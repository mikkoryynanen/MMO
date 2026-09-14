using MMO.Database.Migrations;
using MMO.Database.Shared;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// 1. Get the connection string manually (Aspire injects this)
var accountDbConnectionString = builder.Configuration.GetConnectionString("accountdb");

// 2. Register the context manually so you can access the "MySqlOptions" builder
// builder.AddMySqlDbContext<AccountDbContext>("accountdb");
// builder.AddMySqlDbContext<WorldDbContext>("worlddb");
builder.Services.AddDbContext<AccountDbContext>(options =>
{
    options.UseMySql(accountDbConnectionString, ServerVersion.AutoDetect(accountDbConnectionString), 
        mysqlOptions =>
        {
            // 3. THIS is where you link the migrations assembly
            mysqlOptions.MigrationsAssembly("MMo.Database.Migrations");
            
            // Optional: Enable retry policies (common in Aspire/Cloud apps)
            mysqlOptions.EnableRetryOnFailure();
        });
});

var worldDbConnectionString = builder.Configuration.GetConnectionString("worlddb");
builder.Services.AddDbContext<WorldDbContext>(options =>
{
    options.UseMySql(worldDbConnectionString, ServerVersion.AutoDetect(worldDbConnectionString), 
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