using MessagePack;

namespace MMO.Shared.Packets;

[MessagePackObject]
public class InputPacket: IPacket
{
    [Key(0)]
    public sbyte X { get; set; }
    [Key(1)]
    public sbyte Y { get; set; }
    [Key(2)] 
    public uint Timestamp { get; set; }
}
