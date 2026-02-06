// using System.Net.Sockets;
//
// namespace MMO.WorldServer.Packets;
//
// public class PacketReader
// {
//     private readonly NetworkStream _stream;
//     private readonly byte[] _buffer = new byte[8192];
//     private int _bufferPosition = 0;
//     private int _bufferLength = 0;
//
//     public PacketReader(NetworkStream stream)
//     {
//         _stream = stream;
//     }
//
//     public async Task<(ushort opcode, byte[] payload)?> ReadPacketAsync(CancellationToken ct = default)
//     {
//         // Ensure we have at least the header (4 bytes: length + opcode)
//         if (!await EnsureBytesAsync(4, ct))
//             return null; // Connection closed
//
//         // Read header
//         ushort length = BitConverter.ToUInt16(_buffer, _bufferPosition);
//         ushort opcode = BitConverter.ToUInt16(_buffer, _bufferPosition + 2);
//         
//         // Validate length
//         if (length > 65535 || length < 4) // sanity check
//             throw new InvalidDataException($"Invalid packet length: {length}");
//
//         int payloadLength = length - 4; // subtract header size
//
//         // Ensure we have the complete payload
//         if (!await EnsureBytesAsync(4 + payloadLength, ct))
//             throw new IOException("Connection closed mid-packet");
//
//         // Extract payload
//         byte[] payload = new byte[payloadLength];
//         Array.Copy(_buffer, _bufferPosition + 4, payload, 0, payloadLength);
//
//         // Advance buffer position
//         _bufferPosition += length;
//
//         // Compact buffer if needed
//         if (_bufferPosition >= _bufferLength)
//         {
//             _bufferPosition = 0;
//             _bufferLength = 0;
//         }
//
//         return (opcode, payload);
//     }
//
//     private async Task<bool> EnsureBytesAsync(int requiredBytes, CancellationToken ct)
//     {
//         // Compact buffer if we're past halfway
//         if (_bufferPosition > 0 && _bufferPosition + requiredBytes > _buffer.Length / 2)
//         {
//             int remaining = _bufferLength - _bufferPosition;
//             Array.Copy(_buffer, _bufferPosition, _buffer, 0, remaining);
//             _bufferLength = remaining;
//             _bufferPosition = 0;
//         }
//
//         // Read more data if needed
//         while (_bufferLength - _bufferPosition < requiredBytes)
//         {
//             int bytesRead = await _stream.ReadAsync(
//                 _buffer.AsMemory(_bufferLength, _buffer.Length - _bufferLength), 
//                 ct);
//
//             if (bytesRead == 0)
//                 return false; // Connection closed
//
//             _bufferLength += bytesRead;
//         }
//
//         return true;
//     }
// }