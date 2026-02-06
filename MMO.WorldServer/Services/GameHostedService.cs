using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer.Services;

// TODO See do we use Background Service or Ihostedservice for this
public sealed class GameHostedService : BackgroundService
{
    private readonly IDispatcher _dispatcher;
    private List<Session> _sessions = new();

    // TOOD Dependency Injection
    public GameHostedService(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    protected override async Task<object> ExecuteAsync(CancellationToken stoppingToken)
    {
        // When connection has been established, create session for client and handle connection
        
        var listener = new TcpListener(IPAddress.Any, 24000);
        listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);

                // Create the session and run it
                var session = new Session(_dispatcher, client);
                _sessions.Add(session);
                Task.Run(async  () => await session.RunAsync(stoppingToken));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        foreach (var session in _sessions)
            session.Disconnect();

        return Task.CompletedTask;
    }
}