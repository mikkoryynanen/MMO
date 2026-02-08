using MMO.Shared.Packets;
using MMO.Shared.Packets.Server;

namespace MMO.Shared;

[MessagePack.Union(0, typeof(HelloPacket))]
[MessagePack.Union(1, typeof(InputPacket))]
[MessagePack.Union(2, typeof(PlayerPositionPacket))]
public interface IPacket
{
    // void Write(BinaryWriter writer);
    // void Read(BinaryReader reader);
}