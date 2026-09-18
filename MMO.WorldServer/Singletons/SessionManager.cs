using System.Net.Sockets;
using MMO.Shared.Packets.Server;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer.Singletons;

public class SessionManager : ISessionManager
{
    private Dictionary<Guid, Session> _sessions = new();
    
    private readonly IDispatcher _dispatcher;

    public SessionManager(
        IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public Session CreateSession(TcpClient client)
    {
        var session = new Session(_dispatcher, client);
        session.OnDisconnected += RemoveSession;
        _sessions.Add(session.Id, session);
        Task.Run(session.RunAsync);
        
        // Inform other sessions of a starting session
        foreach (var s in _sessions)
        {

            s.Value.QueuePacket(new PlayerJoinedPacket
            {
                Id =  session.Id,
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
    
    public Session[]  GetSessions()
    {
        return _sessions.Values.ToArray();
    }
}