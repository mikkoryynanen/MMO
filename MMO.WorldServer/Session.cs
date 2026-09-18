using System.Net.Sockets;
using System.Threading.Channels;
using MessagePack;
using MMO.Shared;
using MMO.Shared.Packets.Server;
using MMO.Shared.Serialization;
using MMO.WorldServer.Handlers;
using MMO.WorldServer.Singletons;

namespace MMO.WorldServer;

/// <summary>
/// Container for single player's session
/// </summary>
public class Session
{
    public delegate void DisconnectedHandler(Guid id);

    public event DisconnectedHandler OnDisconnected;

    public bool IsConnected => _client?.Connected ?? false;
    public Guid Id { get; private set; }

    private Channel<IPacket> _outgoingPackets;

    private TcpClient _client;
    private NetworkStream _stream;

    private readonly IDispatcher _dispatcher;
    private CancellationToken _cancellationToken = new();

    public Session(
        IDispatcher dispatcher,
        TcpClient client)
    {
        Id = Guid.NewGuid();

        _outgoingPackets = Channel.CreateUnbounded<IPacket>();

        _dispatcher = dispatcher;
        _client = client;
        _stream = client.GetStream();
    }

    public void QueuePacket(IPacket packet)
    {
        _outgoingPackets.Writer.TryWrite(packet);
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Client connected. Starting session...");

        // TODO This should be in the network layer? this should already be the game world layer
        // TODO Session should only be considered as container class for session relate stuff

        // Outgoing packets are on different thread
        Task.Run(WriteOutgoingPackets);
        Task.Run(PollClient);
        Task.Run(ReadPackets);
    }

    private async Task ReadPackets()
    {
        // Main loop to read packets from the client
        while (!_cancellationToken.IsCancellationRequested)
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
            _dispatcher.Dispatch(opCode, packet, Id);

            Console.WriteLine($"{DateTime.UtcNow.ToString("T")} | Session {Id} read packet {packet.GetType().Name}");
        }
    }

    /// <summary>
    /// Polls the client assigned to session. If client is disconnected, this session should be considered disconnected
    /// </summary>
    private async Task PollClient()
    {
        while (_client.Connected)
        {
            if (_client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] peek = new byte[1];
                if (await _client.Client.ReceiveAsync(peek, SocketFlags.Peek) == 0)
                {
                    Disconnect();
                }
            }

            await Task.Delay(1000);
        }

        Disconnect();
    }

    private Task WriteOutgoingPackets()
    {
        while (_client.Connected)
        {
            while (_outgoingPackets.Reader.Count > 0)
            {
                if (_outgoingPackets.Reader.TryRead(out IPacket outgoingPacket))
                {
                    // TODO Check if the session is active
                    // if (!_client.Connected)
                    //     continue;

                    byte[] body = MessagePackSerializer.Serialize(outgoingPacket);
                    int totalLength = body.Length + 2; // Payload length + 2 bytes for the op code
                    byte[] packet = new byte[4 + totalLength];

                    try
                    {
                        // Header
                        // TODO What if write fails?
                        BitConverter.TryWriteBytes(packet.AsSpan(0, 4), totalLength); // Packet Body Length
                        var opcode = GetOpCode(outgoingPacket);
                        BitConverter.TryWriteBytes(packet.AsSpan(4, 2), (ushort)(opcode ?? 0)); // opcode
                        
                        // Body
                        body.CopyTo(packet.AsSpan(6));
                        
                        // Write to stream
                        _stream.Write(packet);
                        _stream.Flush();
                    }
                    catch (IOException e)
                    {
                        // TODO Client considered disconnected
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
        _stream = null;
        _client = null;
        // TODO Send event to world simulator to remove player?
        // _worldSimulator.RemovePlayer(Id);
        OnDisconnected?.Invoke(Id);
    }

    private OpCodes? GetOpCode(IPacket packet)
    {
        switch (packet)
        {
            case PlayerPositionPacket:
                return OpCodes.Position;
            case PlayerJoinedPacket:
                return OpCodes.NewPlayer;
        }

        return null;
    }

    private async Task ReadStream(NetworkStream stream, byte[] buffer, int amount)
    {
        try
        {
            int bytesRead = 0;
            while (bytesRead < amount)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(bytesRead, amount - bytesRead), _cancellationToken);
                bytesRead += read;
            }
        }
        catch (IOException e)
        {
            // TODO Client considered disconnected
            Disconnect();
        }
    }
}