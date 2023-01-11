namespace Sharposhop.Core.Loading.Jpeg;

public class BitData
{
    private readonly byte[] _bytes;

    public BitData(byte[] data)
    {
        _bytes = data;
        Position = 0;
    }

    public long Position { get; set; }

    public int GetBit()
    {
        var b = (int)_bytes[Position >> 3];
        var s = (int)(7 - (Position & 0x7));
        Position += 1;
        return (b >> s) & 1;
    }

    public ulong GetBitN(int n)
    {
        ulong val = 0;
        for (var i = 0; i < n; i++)
        {
            val = val * 2 + (uint)GetBit();
        }

        return val;
    }
}