// TestClient/Program.cs

using System.Net.Sockets;
using System.Text;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MMO.Shared;
using MMO.Shared.Packets;

var builder = Host.CreateApplicationBuilder(args);
var host = builder.Build();

// Aspire injects the endpoint as: "Endpoints__worldserver__game-port"
// But the easiest way to get it is using the connection string:
var connectionString = builder.Configuration.GetConnectionString("services:world-server:tcp:0"); 
// Result: "tcp://localhost:13000"

var uri = new Uri("tcp://localhost:24000");
using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(uri.Host, uri.Port);


// Console.WriteLine($"Connected to {uri.Host}:{uri.Port}");

var stream = tcpClient.GetStream();
int times = 0;
while (true)
{
    // using var ms = new MemoryStream();
    // using var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: false);
    
    var helloPacket = new HelloPacket() { Number = 69 };
    
    // var envelope = new PacketEnvelope()
    // {
    //     OpCode = (ushort)OpCodes.Test,
    //     // BodyLength =  body.Length,
    //     Body = new HelloPacket() { Number = 69 }
    // };
    // var packet = MessagePackSerializer.Serialize<PacketEnvelope>(envelope);
    var body = MessagePackSerializer.Serialize<IPacket>(helloPacket);
    
    int totalLength = body.Length + 2; // Payload length + 2 bytes for the op code
    byte[] packet = new byte[4 + totalLength];
    
    // Header
    BitConverter.TryWriteBytes(packet.AsSpan(0, 4), totalLength); // Packet Body Length
    BitConverter.TryWriteBytes(packet.AsSpan(4, 2), (ushort)OpCodes.Test); // opcode
    // Body
    body.CopyTo(packet.AsSpan(6));
    
    await stream.WriteAsync(packet);
    await stream.FlushAsync();

    times++;
    Console.WriteLine($"Test program run {times} times");
    
    await Task.Delay(1000);
}
await host.RunAsync();