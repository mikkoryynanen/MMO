using System.Net.Sockets;

namespace MMO.WorldServer.Singletons;

public interface ISessionManager
{
    Session CreateSession(TcpClient client);
}