// using System.Net.Sockets;
// using System.Threading.Channels;
//
// namespace MMO.Shared.Packets;
//
// public class PacketWriter : IAsyncDisposable
// {
//     private readonly NetworkStream _stream;
//     private readonly Channel<byte[]> _sendQueue;
//     private readonly Task _writerTask;
//     private readonly CancellationTokenSource _cts = new();
//
//     public PacketWriter(NetworkStream stream, int queueCapacity = 1000)
//     {
//         _stream = stream;
//         _sendQueue = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(queueCapacity)
//         {
//             FullMode = BoundedChannelFullMode.Wait // or DropOldest/DropNewest
//         });
//         
//         // Start dedicated writer thread
//         _writerTask = Task.Run(WriterLoopAsync);
//     }
//
//     public async ValueTask WritePacketAsync(ushort opcode, byte[] payload)
//     {
//         // Build packet
//         ushort length = (ushort)(4 + payload.Length);
//         byte[] packet = new byte[length];
//         
//         BitConverter.TryWriteBytes(packet.AsSpan(0, 2), length);
//         BitConverter.TryWriteBytes(packet.AsSpan(2, 2), opcode);
//         Array.Copy(payload, 0, packet, 4, payload.Length);
//
//         // Queue for sending (non-blocking)
//         await _sendQueue.Writer.WriteAsync(packet);
//     }
//
//     public ValueTask WritePacketAsync<T>(ushort opcode, T message)
//     {
//         byte[] payload = MessagePackSerializer.Serialize(message);
//         return WritePacketAsync(opcode, payload);
//     }
//
//     private async Task WriterLoopAsync()
//     {
//         try
//         {
//             await foreach (var packet in _sendQueue.Reader.ReadAllAsync(_cts.Token))
//             {
//                 await _stream.WriteAsync(packet, _cts.Token);
//                 // Optional: batch multiple packets before flushing
//                 // await _stream.FlushAsync(_cts.Token);
//             }
//         }
//         catch (OperationCanceledException)
//         {
//             // Normal shutdown
//         }
//         catch (Exception ex)
//         {
//             // Log error, connection probably dead
//             Console.WriteLine($"Writer error: {ex.Message}");
//         }
//     }
//
//     public async ValueTask DisposeAsync()
//     {
//         _sendQueue.Writer.Complete();
//         await _writerTask;
//         _cts.Cancel();
//         _cts.Dispose();
//     }
// }