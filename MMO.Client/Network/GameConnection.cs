using System.Collections.Concurrent;
using System.Net.Sockets;
using MMO.Shared;
using MMO.Shared.Packets;
using MMO.Shared.Packets.Server;

namespace MMO.Client.Network;

public class GameConnection : IAsyncDisposable
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly CancellationTokenSource _cts = new();
    private Task? _readTask;
    private readonly ConcurrentQueue<ReceivedPacket> _inbox = new();

    public bool IsConnected => _client?.Connected ?? false;
    public string? ConnectionError { get; private set; }

    public async Task<bool> ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port, ct);
            _stream = _client.GetStream();
            _readTask = Task.Run(ReadLoopAsync);
            return true;
        }
        catch (Exception ex)
        {
            ConnectionError = ex.Message;
            await DisposeAsync();
            return false;
        }
    }

    public async Task SendHelloAsync(CancellationToken ct = default)
    {
        if (_stream is null || !IsConnected) return;
        await PacketCodec.WriteAsync(_stream, (ushort)OpCodes.Test, new HelloPacket { Number = 1 }, ct);
    }

    public async Task SendInputAsync(sbyte x, sbyte y, CancellationToken ct = default)
    {
        if (_stream is null || !IsConnected) return;
        var packet = new InputPacket
        {
            X = x,
            Y = y,
            Timestamp = (uint)DateTime.UtcNow.Ticks,
        };
        await PacketCodec.WriteAsync(_stream, (ushort)OpCodes.Input, packet, ct);
    }

    public bool TryDequeue(out ReceivedPacket packet) => _inbox.TryDequeue(out packet);

    public int InboxCount => _inbox.Count;

    private async Task ReadLoopAsync()
    {
        var ct = _cts.Token;
        try
        {
            while (!ct.IsCancellationRequested && IsConnected && _stream is not null)
            {
                var (opCode, body) = await PacketCodec.ReadAsync(_stream, ct);
                _inbox.Enqueue(new ReceivedPacket(opCode, body));
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ConnectionError = ex.Message;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        try { _stream?.Close(); } catch { }
        try { _client?.Close(); } catch { }
        _stream = null;
        _client = null;

        if (_readTask is not null)
        {
            try { await _readTask; } catch { }
        }
        _cts.Dispose();
    }
}

public readonly record struct ReceivedPacket(ushort OpCode, IPacket? Body)
{
    public bool IsPosition => OpCode == (ushort)OpCodes.Position && Body is PlayerPositionPacket;
    public bool IsPlayerJoined => OpCode == (ushort)OpCodes.NewPlayer && Body is PlayerJoinedPacket;
}
