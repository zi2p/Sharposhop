namespace Sharposhop.Core.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> ReadPngHeaderBytesAsync(this Stream stream)
    {
        var bytes = new byte[8];
        await stream.ReadAsync(bytes.AsMemory(0, 8));
        return bytes;
    }

    public static void WriteUInt32(this Stream stream, uint value)
    {
        stream.WriteByte((byte)(value >> 24));
        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }
}