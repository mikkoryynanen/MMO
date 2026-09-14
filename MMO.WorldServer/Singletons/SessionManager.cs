using System.Net.Sockets;
using MMO.Database.Shared;
using MMO.Shared.Packets.Server;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer.Singletons;

public class SessionManager : ISessionManager
{
    private Dictionary<Guid, Session> _sessions = new();
    
    private readonly IDispatcher _dispatcher;
    private readonly IWorldSimulator _worldSimulator;

    public SessionManager(
        IDispatcher dispatcher, 
        IWorldSimulator worldSimulator)
    {
        _dispatcher = dispatcher;
        _worldSimulator = worldSimulator;
    }
    
    public Session CreateSession(TcpClient client)
    {
        var session = new Session(_dispatcher, client, _worldSimulator);
        session.OnDisconnected += RemoveSession;
        _sessions.Add(session.Id, session);
        Task.Run(session.RunAsync);
        
        // Inform other sessions of a starting session
        foreach (var s in _sessions)
        {
            if (s.Key == session.Id)
                continue;
            
            s.Value.QueuePacket(new PlayerJoinedPacket
            {
                Username = "New player"
            });
        }

        return session;
    }

    public void RemoveSession(Guid sessionId)
    {
       if (_sessions.Remove(sessionId))
           Console.WriteLine($"Removed session {sessionId}");
    }

    public Session? GetSession(Guid sessionId)
    {
        return _sessions.GetValueOrDefault(sessionId);
    }
}