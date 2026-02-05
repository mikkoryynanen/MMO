namespace MMO.WorldServer.Handlers;

public interface IDispatcher
{
    void Dispatch(ushort opCode, byte[] payload);
}