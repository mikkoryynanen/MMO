using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using MMO.WorldServer.Handlers;

namespace MMO.WorldServer.Services;

// TODO See do we use Background Service or Ihostedservice for this
public sealed class GameHostedService : BackgroundService
{
    private readonly IDispatcher _dispatcher;

    // TOOD Dependency Injection
    public GameHostedService(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    protected override async Task<object> ExecuteAsync(CancellationToken stoppingToken)
    {
        // When connection has been established, create session for client and handle connection
        
        var listener = new TcpListener(IPAddress.Any, 24000);
        listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = listener.AcceptTcpClientAsync(stoppingToken);
                // TODO
                // Create session for client
                // Handle connection

                while (true)
                {
                    // Read received bytes
                    byte[] headerBuffer = new byte[6];
                    await ReadStream(client.Result.GetStream(), headerBuffer, 6);

                    int payloadLength = BitConverter.ToInt32(headerBuffer, 0) - 2; // 2 bytes for op code
                    ushort opCode = BitConverter.ToUInt16(headerBuffer, 4);
                    
                    byte[] payload = new byte[payloadLength];
                    if (payloadLength > 0)
                    {
                        await ReadStream(client.Result.GetStream(), payload, payloadLength);
                    }
                    
                    // var session = new Session();
                    
                    _dispatcher.Dispatch(opCode, payload);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return Task.CompletedTask;
    }

    private async Task ReadStream(NetworkStream stream, byte[] buffer, int amount)
    {
       int bytesRead = 0;
       while (bytesRead < amount)
       {
           int read = await stream.ReadAsync(buffer.AsMemory(bytesRead, amount - bytesRead), CancellationToken.None);
           bytesRead += read;
       }
    }
}