using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MMO.WorldServer.Handlers;
using MMO.WorldServer.Services;
using MMO.WorldServer.Singletons;

namespace MMO.WorldServer;

internal class Program
{
    private static readonly CancellationTokenSource _cancellationToken = new();
    
    static async Task Main(string[] args)
    {
        IHostBuilder builder = new HostBuilder()
            .ConfigureServices(sc =>
            {
                sc.AddSingleton<IDispatcher, HandlerDispatcher>();
                sc.AddSingleton<IWorldSimulator, WorldSimulator>();
                sc.AddSingleton<ISessionManager, SessionManager>();
                
                sc.AddHostedService<GameHostedService>();
            });

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
