using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using MMO.Database.Shared;
using MMO.Shared.Packets;
using MMO.Shared.Packets.Server;
using MMO.WorldServer.Map;

namespace MMO.WorldServer.Singletons;

public struct PlayerInput
{
    public Guid SessionId { get; set; }
    public InputPacket InputPacket { get; set; }
    public uint Timestamp { get; set; }
}

// TODO should be inside session
public class Player
{
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

    private readonly Channel<PlayerInput> _inputQueue = Channel.CreateUnbounded<PlayerInput>();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly WorldDbContext _worldDbContext;
    private readonly WorldMap _worldMap = new();

    public WorldSimulator(WorldDbContext worldDbContext)
    {
        _worldDbContext = worldDbContext;
    }

    public void Start()
    {
        _simulationTask = Task.Run(SimulationLoop);
        
        _worldMap.Populate(_worldDbContext.WorldEntities.ToArray());

        IsRunning = true;
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    public void AddPlayer(Session session)
    {
        // TODO We could be adding sessions here and contain player inside session
        _players.TryAdd(session.Id, new Player
        {
            Session = session
        });
    }

    public void RemovePlayer(Guid sessionId)
    {
        if (_players.TryRemove(sessionId, out var player))
        {
            Console.WriteLine($"Removed player");
            // TODO Remove this players inputs as well? do they get removed from inputQueue

            // while (_inputQueue.Reader.TryPeek(out var peekedInput))
            // {
            //     if (peekedInput.SessionId == sessionId)
            //     {
            //         if (!_inputQueue.Reader.TryDequeue(out _))
            //             Console.WriteLine("Failed to remove inputs");
            //     }
            // }
        }
    }

    public void EnqueueInput(PlayerInput input)
    {
        // _inputQueue.Enqueue(input);
        _inputQueue.Writer.TryWrite(input);
    }

    public bool HasInput(InputPacket input)
    {
        return _inputQueue.Reader.TryPeek(out PlayerInput peekedInput) &&
               peekedInput.InputPacket.X == input.X &&
               peekedInput.InputPacket.Y == input.Y;
    }

    private async Task SimulationLoop()
    {
        var stopwatch = Stopwatch.StartNew();
        long nextTick = stopwatch.ElapsedMilliseconds + TICK_MS;

        long sleeptime = nextTick - stopwatch.ElapsedMilliseconds;
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            Console.WriteLine("World tick");
            
            // TODO, add try catch here
            try
            {

            
                // Process input queue
                while (_inputQueue.Reader.TryRead(out var input))
                {
                    // Console.WriteLine($"{DateTime.UtcNow.ToString("T")} | Clearing input queue");
                    if (input.InputPacket.X == 0 && input.InputPacket.Y == 0)
                        continue;
                
                    // var now = stopwatch.ElapsedMilliseconds;
                    var deltaTime = TICK_MS / 1000.0f;
                
                    // Do not process disconnected players inputs
                    if (!_players.ContainsKey(input.SessionId))
                        continue;

                    // var ms = input.Timestamp - input.InputPacket.Timestamp;

                    if (!_players.TryGetValue(input.SessionId, out var player))
                        continue;

                    // Update player position
                    var requestedPositionX = player.X + input.InputPacket.X * 200f * deltaTime;
                    var requestedPositonY = player.Y + input.InputPacket.Y * 200f * deltaTime;
                    if (!_worldMap.IsColliding(requestedPositionX, requestedPositonY))
                    {
                        player.X = requestedPositionX;
                        player.Y = requestedPositonY;
                        // Console.WriteLine($"input x: {input.InputPacket.X}, y: {input.InputPacket.Y}");

                        // Queue the position packets to send to other clients via the session class
                        // TODO Quantize position values
                        player.Session.QueuePacket(new PlayerPositionPacket
                        {
                            X = player.X,
                            Y = player.Y
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // TODO Update player position on the map grid

            // Sleep until next tick
            nextTick += TICK_MS;
            if (sleeptime > 0)
                await Task.Delay((int)sleeptime, _cancellationTokenSource.Token);
            else
                Console.WriteLine($"World TICK is lagging by {-sleeptime}ms");
        }

        Console.WriteLine($"World simulator shut down");
    }
}