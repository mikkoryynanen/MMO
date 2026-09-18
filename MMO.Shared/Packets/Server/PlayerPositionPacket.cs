using MessagePack;

namespace MMO.Shared.Packets.Server;

[MessagePackObject]
public class PlayerPositionPacket : IPacket
{
    [Key(0)] 
    public Guid Id  { get; set; }
    
    [Key(1)] 
    public float X { get; set; }
    
    [Key(2)] 
    public float Y { get; set; }
}