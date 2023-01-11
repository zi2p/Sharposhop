namespace Sharposhop.Core.Loading.Jpeg.Segments;

public class QuantizationTable
{
    public static readonly byte[] Marker = { 0xFF, 0xDB };

    public QuantizationTable(byte[] data)
    {
        Length = data[0] << 8 | data[1];
        NumberOfTable = data[2] & 0x0F;
        Precision = (data[2] & 0xF0) >> 4 == 0 ? Precision.Bit8 : Precision.Bit16;
        Values = data[3..];
    }

    public int Length { get; }
    public int NumberOfTable { get; }
    public Precision Precision { get; }
    public byte[] Values { get; }
}

public enum Precision
{
    Bit8,
    Bit16
}