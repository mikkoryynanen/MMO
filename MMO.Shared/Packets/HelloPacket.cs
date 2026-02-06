using MessagePack;
using MMO.Shared;

[MessagePackObject]
public class HelloPacket: IPacket
{
    // public string Message { get; set; }
    [Key(0)]
    public int Number { get; set; }
    
    // public void Write(BinaryWriter writer)
    // {
    //     writer.Write(Number);
    // }
    //
    // public void Read(BinaryReader reader)
    // {
    //     Number = reader.ReadInt32();
    // }
}
