using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using MMO.WorldServer.Handlers;
using MMO.WorldServer.Singletons;

namespace MMO.WorldServer.Services;

// TODO See do we use Background Service or Ihostedservice for this
public sealed class GameHostedService : BackgroundService
{
    private readonly IDispatcher _dispatcher;
    private readonly IWorldSimulator _worldSimulator;
    private readonly ISessionManager _sessionManager;

    public GameHostedService(IDispatcher dispatcher, IWorldSimulator worldSimulator, ISessionManager sessionManager)
    {
        _dispatcher = dispatcher;
        _worldSimulator = worldSimulator;
        _sessionManager = sessionManager;
    }
    
    protected override async Task<object> ExecuteAsync(CancellationToken stoppingToken)
    {
        // When connection has been established, create session for client and handle connection
        
        var listener = new TcpListener(IPAddress.Any, 24000);
        listener.Start();
        
        // Run the world simulator, handled its own simulation thread
        _worldSimulator.Start(_sessionManager);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                
                var session = _sessionManager.CreateSession(client);
                _worldSimulator.AddPlayer(session);
                
                // Architecture should look like this:
                // (this)HostedService - Orchestrates the following layers
                // NetworkLayer Thread - Read and write packets to/from clients and dispatches them to the correct handlers aka GameWorld thread
                // GameWorld Thread - Ticks the game state and sends updates to NetworkLayer to send back to clients
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return Task.CompletedTask;
    }
}