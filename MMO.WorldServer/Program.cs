using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MMO.Database.Shared;
using MMO.WorldServer.Handlers;
using MMO.WorldServer.Services;
using MMO.WorldServer.Singletons;

namespace MMO.WorldServer;

internal class Program
{
    private static readonly CancellationTokenSource _cancellationToken = new();
    
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<IDispatcher, HandlerDispatcher>();
        builder.Services.AddSingleton<IWorldSimulator, WorldSimulator>();
        builder.Services.AddSingleton<ISessionManager, SessionManager>();
        builder.Services.AddHostedService<GameHostedService>();
        builder.AddMySqlDbContext<WorldDbContext>("worlddb");

        try
        {
            var host = builder.Build();
            await host.RunAsync(_cancellationToken.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
