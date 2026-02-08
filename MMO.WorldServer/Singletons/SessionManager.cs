using System.Net.Sockets;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer.Singletons;

public class SessionManager : ISessionManager
{
    private List<Session> _sessions = new();
    
    private readonly IDispatcher _dispatcher;

    public SessionManager(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public Session CreateSession(TcpClient client)
    {
        var session = new Session(_dispatcher, client);
        _sessions.Add(session);
        Task.Run(() => session.RunAsync(CancellationToken.None));

        return session;
    }
}