using MessagePack;

namespace MMO.Shared.Packets.Server;

[MessagePackObject]
public class PlayerJoinedPacket : IPacket
{
    [Key(0)] 
    public Guid Id { get; set; }
    
    [Key(1)] 
    public string Username { get; set; }
}

