using System.Net.Sockets;
using System.Threading.Channels;
using MessagePack;
using MMO.Shared;
using MMO.Shared.Serialization;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer;

public class Session
{
    public bool IsConnected => _client?.Connected ?? false;

    private Channel<IPacket> _outgoingPackets; 
    
    private TcpClient _client;
    private NetworkStream _stream;
    
    private readonly IDispatcher _dispatcher;

    public Session(IDispatcher dispatcher, TcpClient client)
    {
        _outgoingPackets = Channel.CreateUnbounded<IPacket>();
        
        _dispatcher = dispatcher;
        _client = client;
        _stream = client.GetStream();
    }

    public void QueuePacket(IPacket packet)
    {
        _outgoingPackets.Writer.TryWrite(packet);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Client connected. Starting session...");
        
        // TODO This should be in the network layer? this should already be the game world layer
        // TODO Session should only be considered as container class for session relate stuff
        
        // Outgoing packets are on different thread
        Task.Run(() =>
        {
            while (true)
            {
                while (_outgoingPackets.Reader.Count > 0)
                {
                    if (_outgoingPackets.Reader.TryRead(out IPacket outgoingPacket))
                    {
                        // TODO Check if the session is active
                        // if (!_client.Connected)
                        //     continue;
                        
                        var body = MessagePackSerializer.Serialize<IPacket>(outgoingPacket);

                        int totalLength = body.Length + 2; // Payload length + 2 bytes for the op code
                        byte[] packet = new byte[4 + totalLength];

                        // Header
                        BitConverter.TryWriteBytes(packet.AsSpan(0, 4), totalLength); // Packet Body Length
                        BitConverter.TryWriteBytes(packet.AsSpan(4, 2), (ushort)OpCodes.Position); // opcode
                        // Body
                        body.CopyTo(packet.AsSpan(6));
                        
                        _stream.Write(packet);
                        _stream.Flush();
                    }
                }
            }
        });
        
        // Main loop to read packets from the client
        while (!cancellationToken.IsCancellationRequested)
        {
            // TODO Could us channel here, now we are waiting for all bytes to arrive from the packet before moving ahead
           // TODO Should do this least once the header is read 
            
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
                    
            var packet = Serializer.Deserialize<IPacket>(payload);
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