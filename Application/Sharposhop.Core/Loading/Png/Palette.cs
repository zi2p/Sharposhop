namespace Sharposhop.Core.Loading.Png;

public class Palette
{
    public Palette(byte[] data)
    {
        Data = new byte[data.Length];
        var dataIndex = 0;
        for (var i = 0; i < data.Length; i += 3)
        {
            Data[dataIndex++] = data[i];
            Data[dataIndex++] = data[i + 1];
            Data[dataIndex++] = data[i + 2];
        }
    }

    public byte[] Data { get; }

    public (byte r, byte g, byte b) GetColor(int index)
    {
        var start = index * 3;
        return (Data[start], Data[start + 1], Data[start + 2]);
    }
}