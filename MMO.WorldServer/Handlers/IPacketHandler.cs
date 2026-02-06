using MMO.Shared;

namespace MMO.WorldServer.Handlers;

public interface IPacketHandler
{
    void Handle(IPacket packet);
}