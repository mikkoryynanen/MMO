namespace MMO.Shared;

[MessagePack.Union(0, typeof(HelloPacket))]
public interface IPacket
{
    // void Write(BinaryWriter writer);
    // void Read(BinaryReader reader);
}