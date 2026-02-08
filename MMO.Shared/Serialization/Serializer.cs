namespace MMO.Shared.Serialization;

public static class Serializer
{
    public static void Serialize<T>(T data, Stream stream)
    {
        MessagePack.MessagePackSerializer.Serialize(stream, data);
    }
    
    public static T Deserialize<T>(Stream stream)
    {
        return MessagePack.MessagePackSerializer.Deserialize<T>(stream);
    }
    
    public static T Deserialize<T>(byte[] data)
    {
        return MessagePack.MessagePackSerializer.Deserialize<T>(data);
    }
}