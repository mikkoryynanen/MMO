using System.Net.Sockets;
using MessagePack;
using MMO.Shared;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer;

public class Session
{
    public bool IsConnected => _client?.Connected ?? false;
    
    private TcpClient _client;
    private NetworkStream _stream;
    
    private readonly IDispatcher _dispatcher;

    public Session(IDispatcher dispatcher, TcpClient client)
    {
        _dispatcher = dispatcher;
        _client = client;
        _stream = client.GetStream();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Client connected. Starting session...");
        
        // Main loop to read packets from the client
        while (!cancellationToken.IsCancellationRequested)
        {
            // Read received bytes
            byte[] headerBuffer = new byte[6];
            await ReadStream(_stream, headerBuffer, 6);
                    
            int payloadLength = BitConverter.ToInt32(headerBuffer, 0) - 2; // 2 bytes for op code
            ushort opCode = BitConverter.ToUInt16(headerBuffer, 4);
                    
            byte[] payload = new byte[payloadLength];
            if (payloadLength > 0)
            {
                await ReadStream(_stream, payload, payloadLength);
            }
                    
            var packet = MessagePackSerializer.Deserialize<IPacket>(payload);
            _dispatcher.Dispatch(opCode, packet);
        }
    }
    
    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
        _stream = null;
        _client = null;
    }
    
    private async Task ReadStream(NetworkStream stream, byte[] buffer, int amount)
    {
        int bytesRead = 0;
        while (bytesRead < amount)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(bytesRead, amount - bytesRead), CancellationToken.None);
            bytesRead += read;
        }
    }
}