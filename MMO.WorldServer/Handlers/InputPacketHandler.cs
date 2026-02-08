using MMO.Shared;
using MMO.Shared.Packets;
using MMO.WorldServer.Singletons;

namespace MMO.WorldServer.Handlers;

public class InputPacketHandler : IPacketHandler
{
    private readonly IWorldSimulator _worldSimulator;

    public InputPacketHandler(IWorldSimulator worldSimulator)
    {
       _worldSimulator = worldSimulator;
    }
    
    public void Handle(IPacket packet)
    {
        var input = (InputPacket)packet;

        // if (_worldSimulator.HasInput(input))
        //     return;
        
        // TODO Build the PlayerInput packet to enqueue
        var playerInput = new PlayerInput()
        {
            InputPacket = input,
            PlayerId = Guid.Empty,   // TODO This clients player id
            // Represents the Servers timestamp
            Timestamp = (uint)DateTime.UtcNow.Ticks
        };
        _worldSimulator.EnqueueInput(playerInput);
    }
}