using System.Net.Sockets;
using MessagePack;
using MMO.Shared;

namespace MMO.Client.Network;

public static class PacketCodec
{
    public const int HeaderSize = 6;

    public static async ValueTask WriteAsync(
        NetworkStream stream,
        ushort opCode,
        IPacket body,
        CancellationToken ct = default)
    {
        var payload = MessagePackSerializer.Serialize<IPacket>(body);
        var buffer = new byte[HeaderSize + payload.Length];

        BitConverter.TryWriteBytes(buffer.AsSpan(0, 4), payload.Length + 2);
        BitConverter.TryWriteBytes(buffer.AsSpan(4, 2), opCode);
        payload.CopyTo(buffer.AsSpan(HeaderSize));

        await stream.WriteAsync(buffer, ct);
    }

    public static async ValueTask<(ushort opCode, IPacket? body)> ReadAsync(
        NetworkStream stream,
        CancellationToken ct = default)
    {
        var header = new byte[HeaderSize];
        await ReadExactAsync(stream, header, HeaderSize, ct);

        int bodyLength = BitConverter.ToInt32(header, 0) - 2;
        ushort opCode = BitConverter.ToUInt16(header, 4);

        if (bodyLength <= 0)
            return (opCode, null);

        var payload = new byte[bodyLength];
        await ReadExactAsync(stream, payload, bodyLength, ct);
        return (opCode, MessagePackSerializer.Deserialize<IPacket>(payload));
    }

    private static async ValueTask ReadExactAsync(
        NetworkStream stream,
        byte[] buffer,
        int amount,
        CancellationToken ct)
    {
        int read = 0;
        while (read < amount)
        {
            int n = await stream.ReadAsync(buffer.AsMemory(read, amount - read), ct);
            if (n == 0)
                throw new EndOfStreamException();
            read += n;
        }
    }
}
