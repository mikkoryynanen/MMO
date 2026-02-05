// TestClient/Program.cs

using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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

int times = 0;
while (true)
{
    // Now use the DummyClient logic we discussed:
    ushort pingOp = 33603;
    Console.WriteLine($"Sending message with op ID {pingOp}");

    var stream = tcpClient.GetStream();

    using var ms = new MemoryStream();
    using (var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) {
        writer.Write(6);    // 4 bytes
        writer.Write(9);    // 4 bytes
    }
    byte[] body = ms.ToArray();

    int totalLength = body.Length + 2; // Payload length + 2 bytes for the op code
    byte[] packet = new byte[4 + totalLength];
    
    // Header
    BitConverter.TryWriteBytes(packet.AsSpan(0, 4), totalLength); // Packet Body Length
    BitConverter.TryWriteBytes(packet.AsSpan(4, 2), pingOp); // opcode
    // Body
    body.CopyTo(packet.AsSpan(6));
    
    await stream.WriteAsync(packet);
    await stream.FlushAsync();

    times++;
    Console.WriteLine($"Test program run {times} times");
    
    await Task.Delay(1000);
}
await host.RunAsync();