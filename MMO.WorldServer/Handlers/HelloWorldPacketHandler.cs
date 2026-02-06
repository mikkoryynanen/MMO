using MMO.Shared;

namespace MMO.WorldServer.Handlers;

public class HelloWorldPacketHandler : IPacketHandler
{
    public void Handle(IPacket packet)
    {
        var data = (HelloPacket)packet;
    }
}