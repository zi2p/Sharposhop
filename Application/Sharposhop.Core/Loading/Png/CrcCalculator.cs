namespace Sharposhop.Core.Loading.Png;

public class CrcCalculator
{
    private const uint Polynom = 0xEDB88320;
    private static readonly uint[] Table;

    static CrcCalculator()
    {
        Table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            var value = i;
            for (var j = 0; j < 8; ++j)
            {
                if ((value & 1) != 0)
                    value = (value >> 1) ^ Polynom;
                else
                    value >>= 1;
            }

            Table[i] = value;
        }
    }

    public static uint CalculateCrc(byte[] data)
    {
        var crc = uint.MaxValue;
        foreach (var t in data)
        {
            var index = (crc ^ t) & 0xFF;
            crc = (crc >> 8) ^ Table[index];
        }

        return crc ^ uint.MaxValue;
    }

    public static uint CalculateCrc(byte[] data1, byte[] data2)
    {
        var crc = uint.MaxValue;
        foreach (var t in data1)
        {
            var index = (crc ^ t) & 0xFF;
            crc = (crc >> 8) ^ Table[index];
        }

        foreach (var t in data2)
        {
            var index = (crc ^ t) & 0xFF;
            crc = (crc >> 8) ^ Table[index];
        }

        return crc ^ uint.MaxValue;
    }
}