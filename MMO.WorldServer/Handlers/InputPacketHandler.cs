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
    
    public void Handle(IPacket packet, Guid sessionId)
    {
        var input = (InputPacket)packet;
        
        // TODO Build the PlayerInput packet to enqueue
        var playerInput = new PlayerInput
        {
            InputPacket = input,
            SessionId =  sessionId,
            // Represents the Servers timestamp
            Timestamp = (uint)DateTime.UtcNow.Ticks
        };
        _worldSimulator.EnqueueInput(playerInput);
    }
}