using MMO.Shared;

namespace MMO.WorldServer.Handlers;

public class HelloWorldPacketHandler : IPacketHandler
{
    public void Handle(IPacket packet, Guid sessionId)
    {
        // var data = (HelloPacket)packet;
        // Console.WriteLine($"Hello packet handled with number: {data.Number}");
    }
}