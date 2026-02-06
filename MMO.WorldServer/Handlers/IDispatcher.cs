using MMO.Shared;

namespace MMO.WorldServer.Handlers;

public interface IDispatcher
{
    void Dispatch(ushort opcode, IPacket packet);
}