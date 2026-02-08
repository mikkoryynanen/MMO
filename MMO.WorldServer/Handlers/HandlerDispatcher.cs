using MMO.Shared;
using MMO.WorldServer.Singletons;

namespace MMO.WorldServer.Handlers;

public class HandlerDispatcher : IDispatcher
{
    private Dictionary<ushort, IPacketHandler> _handlerTypes = new();
    private readonly IWorldSimulator _worldSimulator;

    public HandlerDispatcher(IWorldSimulator worldSimulator)
    {
        _worldSimulator = worldSimulator;
        
        RegisterHandlers();
    }

    public void Dispatch(ushort opCode, IPacket packet)
    {
        if (!_handlerTypes.ContainsKey(opCode))
        {
            Console.WriteLine($"Cannot find handler for opcode: {opCode}");
        }
        else
        {
            _handlerTypes[opCode].Handle(packet);
        }
    }

    private void RegisterHandlers()
    {
        _handlerTypes[(ushort)OpCodes.Test] = new HelloWorldPacketHandler();
        _handlerTypes[(ushort)OpCodes.Input] = new InputPacketHandler(_worldSimulator);
        
        // var handlers = AppDomain.CurrentDomain.GetAssemblies()
        //     .SelectMany(s => s.GetTypes())
        //     .Where(p => typeof(IPacketHandler).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);
        // foreach (var handlerType in handlers)
        // {
        //     var opcode = GetStableOpcode(handlerType.FullName);
        //     // TODO Could build this better, without using reflection, but this is good enough for now
        //     _handlerTypes[opcode] = (IPacketHandler)Activator.CreateInstance(handlerType)!;
        // }
    }
    
    // TODO Move to utils
    // TODO Could replace this logic with handshake manifest file, where the server provides the correct list of opcodes to the client
    // public static ushort GetStableOpcode(string typeName)
    // {
    //     uint hash = 2166136261;
    //     foreach (byte b in Encoding.UTF8.GetBytes(typeName.ToLowerInvariant()))
    //     {
    //         hash = (hash ^ b) * 16777619;
    //     }
    //     
    //     // Fold the 32-bit hash into a 16-bit ushort
    //     // This XORs the high 16 bits with the low 16 bits to maintain entropy
    //     return (ushort)((hash >> 16) ^ (hash & 0xFFFF));
    // }
}