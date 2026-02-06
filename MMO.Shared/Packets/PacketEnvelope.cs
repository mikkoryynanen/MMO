using MessagePack;

namespace MMO.Shared.Packets;

[MessagePackObject]
public class PacketEnvelope
{
   [Key(0)]
   public ushort OpCode { get; set; }
   // [Key(1)]
   // public int BodyLength { get; set; }
   [Key(1)]
   public IPacket Body { get; set; } 
}