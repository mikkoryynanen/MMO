namespace MMO.WorldServer.Handlers;

public interface IPacketHandler
{
    void Handle(byte[] payload);
}