using MMO.Shared.Packets;
using MMO.Shared.Packets.Server;

namespace MMO.Shared;

// TODO This fucking list is horrible, just been wasting time on it, need to find a better way to do this
[MessagePack.Union(0, typeof(HelloPacket))]
[MessagePack.Union(1, typeof(InputPacket))]
[MessagePack.Union(2, typeof(PlayerPositionPacket))]
[MessagePack.Union(3, typeof(PlayerJoinedPacket))]
public interface IPacket
{
}