using System.Net;
using System.Net.Sockets;

namespace MMO.WorldServer.Singletons;

public interface ISessionManager
{
    Session CreateSession(TcpClient client);
    void RemoveSession(Guid sessionId);
    Session? GetSession(Guid sessionId);
}