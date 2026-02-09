using MMO.Database.Migrations;

var builder = Host.CreateApplicationBuilder(args);

builder.AddMySqlDbContext<GameDbContext>("accountdb");

builder.Services.AddHostedService<MigrationWorker>();

var app = builder.Build();
app.Run();