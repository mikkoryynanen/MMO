using System.Text;

namespace MMO.WorldServer.Handlers;

public class HandlerDispatcher : IDispatcher
{
    private Dictionary<ushort, Type> _handlerTypes = new();

    public HandlerDispatcher()
    {
        RegisterHandlers();
    }

    public void Dispatch(ushort opCode, byte[] payload)
    {
        _handlerTypes[opCode].GetConstructor(Type.EmptyTypes)?.Invoke(null);
    }

    private void RegisterHandlers()
    {
        var handlers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IPacketHandler).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);
        foreach (var handlerType in handlers)
        {
            var opcode = GetStableOpcode(handlerType.FullName);
            _handlerTypes[opcode] = handlerType;
        }
    }
    
    // TODO Move to utils
    // TODO Could replace this logic with handshake manifest file, where the server provides the correct list of opcodes to the client
    public static ushort GetStableOpcode(string typeName)
    {
        uint hash = 2166136261;
        foreach (byte b in Encoding.UTF8.GetBytes(typeName.ToLowerInvariant()))
        {
            hash = (hash ^ b) * 16777619;
        }
        
        // Fold the 32-bit hash into a 16-bit ushort
        // This XORs the high 16 bits with the low 16 bits to maintain entropy
        return (ushort)((hash >> 16) ^ (hash & 0xFFFF));
    }
}