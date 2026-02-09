using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using MMO.Shared.Packets;
using MMO.Shared.Packets.Server;

namespace MMO.WorldServer.Singletons;

public struct PlayerInput
{
    // TODO Not the same as session id, this should be fetched from authentication flow
    // NOTE Using session id for now
    public Guid PlayerId { get; set; }
    public InputPacket InputPacket { get; set; }
    public uint Timestamp { get; set; }
}

public class Player
{
    public Guid Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public Session Session { get; set; }
}

public class WorldSimulator : IWorldSimulator
{
    private const int SIMULATION_TICK_RATE = 30; // Ticks per second
    private const int TICK_MS = 1000 / SIMULATION_TICK_RATE;

    public bool IsRunning { get; private set; }

    private Task _simulationTask;

    private readonly ConcurrentDictionary<Guid, Player> _players = new();

    // private readonly Channel<PlayerInput> _inputChannel;
    private readonly Queue<PlayerInput> _inputQueue = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public WorldSimulator()
    {
        // _inputChannel = Channel.CreateUnbounded<PlayerInput>();
    }

    public void Start()
    {
        _simulationTask = Task.Run(SimulationLoop);

        IsRunning = true;
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    public void AddPlayer(Session session)
    {
        var id = Guid.Empty;
        _players.TryAdd(id, new Player()
        {
            Id = id,
            Session = session
        });
    }

    public void EnqueueInput(PlayerInput input)
    {
        _inputQueue.Enqueue(input);
        // _inputQueue.Writer.TryWrite(input);
    }

    public bool HasInput(InputPacket input)
    {
        return _inputQueue.TryPeek(out var peekedInput) &&
               peekedInput.InputPacket.X == input.X &&
               peekedInput.InputPacket.Y == input.Y;
    }

    private async Task SimulationLoop()
    {
        var stopwatch = Stopwatch.StartNew();
        long nextTick = stopwatch.ElapsedMilliseconds + TICK_MS;

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            long now = stopwatch.ElapsedMilliseconds;
            float deltaTime = TICK_MS / 1000.0f;

            // Clean disconnected players
            CleanDisconnectedPlayers();

            // Process input queue
            if (_inputQueue.TryDequeue(out PlayerInput input))
            {
                // Do not process disconnected players inputs
                if (!_players.ContainsKey(input.PlayerId))
                    continue;

                var ms = input.Timestamp - input.InputPacket.Timestamp;

                // TODO Enable this once we have auth server running
                if (!_players.TryGetValue(input.PlayerId, out var player))
                    continue;

                // TODO Is is the correct player? since we are not disconnecting players
                // Update player position
                player.X += input.InputPacket.X * 200f * deltaTime;
                player.Y += input.InputPacket.Y * 200f * deltaTime;

                Console.WriteLine($"input x: {input.InputPacket.X}, y: {input.InputPacket.Y}");

                // TODO Quantize position values
                player.Session.QueuePacket(new PlayerPositionPacket
                {
                    X = player.X,
                    Y = player.Y
                });
            }

            // TODO Update player position on the map grid

            // Sleep until next tick
            nextTick += TICK_MS;
            long sleeptime = nextTick - stopwatch.ElapsedMilliseconds;
            if (sleeptime > 0)
                await Task.Delay((int)sleeptime, _cancellationTokenSource.Token);
            else
                Console.WriteLine($"World TICK is lagging by {-sleeptime}ms");
        }
    }

    private void CleanDisconnectedPlayers()
    {
        if (_players.Count == 0)
            return;
        
        var disconnectedPlayers = new List<Player>();
        foreach (var p in _players.Values)
        {
            if (!p.Session.IsConnected)
            {
                disconnectedPlayers.Add(p);
            }
        }

        foreach (var dcp in disconnectedPlayers)
        {
            _players.TryRemove(dcp.Id, out _);
        }
    }
}