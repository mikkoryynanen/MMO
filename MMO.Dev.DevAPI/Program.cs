using MMO.Database.Shared;
using MMO.Dev.WorldAPI.Endpoints;
using MMO.Dev.WorldAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddMySqlDbContext<WorldDbContext>("worlddb");

builder.Services.AddScoped<DatabaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapWorldEndpoints();

app.Run();