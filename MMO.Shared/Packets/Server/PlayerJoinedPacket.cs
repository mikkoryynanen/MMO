using MessagePack;

namespace MMO.Shared.Packets.Server;

[MessagePackObject]
public class PlayerJoinedPacket : IPacket
{
    [Key(0)] 
    public string Username { get; set; }
}

