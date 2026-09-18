using MMO.Client.Entities;
using MMO.Client.Network;
using MMO.Client.Render;
using MMO.Shared.Packets.Server;

namespace MMO.Client.Game;

public enum ClientMode { Login, InGame }

public class GameClient : IAsyncDisposable
{
    private const int InputSendHz = 20;
    private const int InputSendMs = 1000 / InputSendHz;
    private static readonly TimeSpan LoginTimeout = TimeSpan.FromSeconds(10);

    public ClientMode Mode { get; private set; } = ClientMode.Login;
    public GameState State { get; } = new();
    public InputState Input { get; } = new();

    public AuthClient Auth { get; }
    public GameConnection? Connection { get; private set; }

    public SessionToken? CurrentToken { get; private set; }

    private readonly string _worldHost;
    private readonly int _worldPort;
    private TimeSpan _inputAccumulator;

    public GameClient(AuthClient auth, string worldHost, int worldPort)
    {
        Auth = auth;
        _worldHost = worldHost;
        _worldPort = worldPort;
    }

    public async Task<bool> TryLoginAsync(string username, string password, CancellationToken ct)
    {
        using var timeoutCts = new CancellationTokenSource(LoginTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        CurrentToken = await Auth.LoginAsync(username, password, linkedCts.Token);
        if (CurrentToken is null) return false;
        return await ConnectToWorldAsync(username, linkedCts.Token);
    }

    public async Task<bool> TryRegisterAsync(string username, string password, CancellationToken ct)
    {
        using var timeoutCts = new CancellationTokenSource(LoginTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        var ok = await Auth.RegisterAsync(username, password, linkedCts.Token);
        if (!ok) return false;
        return await TryLoginAsync(username, password, linkedCts.Token);
    }

    private async Task<bool> ConnectToWorldAsync(string username, CancellationToken ct)
    {
        Connection ??= new GameConnection();
        var ok = await Connection.ConnectAsync(_worldHost, _worldPort, ct);
        if (!ok) return false;

        State.Self.Username = username;
        State.Self.X = Renderer.WorldWidth / 2f;
        State.Self.Y = Renderer.WorldHeight / 2f;

        await Connection.SendHelloAsync(ct);
        Mode = ClientMode.InGame;
        return true;
    }

    public async Task DisconnectAsync()
    {
        if (Connection is not null)
        {
            await Connection.DisposeAsync();
            Connection = null;
        }
        Mode = ClientMode.Login;
    }

    public void Update(float dt, bool escPressed)
    {
        if (Mode != ClientMode.InGame || Connection is null) return;

        Input.Poll();
        var (x, y) = Input.Vector();

        _inputAccumulator += TimeSpan.FromSeconds(dt);
        if (_inputAccumulator.TotalMilliseconds >= InputSendMs)
        {
            _inputAccumulator = TimeSpan.Zero;
            // Fire-and-forget — we don't want to block the render loop on the network write.
            _ = Connection.SendInputAsync(x, y);
        }

        DrainInbox();

        if (escPressed)
            _ = DisconnectAsync();
    }

    private void DrainInbox()
    {
        if (Connection is null) return;
        while (Connection.TryDequeue(out var received))
        {
            if (received.IsPosition && received.Body is PlayerPositionPacket pos)
            {
                State.ApplyRemotePosition(pos.Id, pos.X, pos.Y);
            }
            else if (received.IsPlayerJoined && received.Body is PlayerJoinedPacket join)
            {
                State.ApplyRemoteJoin(join.Id, join.Username);
            }
            else if (received.Body is not null)
            {
                Console.WriteLine($"[Client] Unhandled packet opcode=0x{received.OpCode:X4} type={received.Body.GetType().Name}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Connection is not null)
        {
            await Connection.DisposeAsync();
            Connection = null;
        }
    }
}
