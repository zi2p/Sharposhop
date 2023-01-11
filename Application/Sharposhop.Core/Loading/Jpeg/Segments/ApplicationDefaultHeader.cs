using System.Text;

namespace Sharposhop.Core.Loading.Jpeg.Segments;

public class ApplicationDefaultHeader
{
    public static readonly byte[] Marker = { 0xFF, 0xE0 };

    public ApplicationDefaultHeader(byte[] data)
    {
        Length = data[0] << 8 | data[1];
        Identifier = Encoding.ASCII.GetString(data, 2, 5);
        Version = $"{data[7]}.{data[8]}";
        Units = data[9];
        XDensity = data[10] << 8 | data[11];
        YDensity = data[12] << 8 | data[13];
        ThumbnailWidth = data[14];
        ThumbnailHeight = data[15];
    }

    public int Length { get; }
    public string Identifier { get; }
    public string Version { get; }
    public int Units { get; }
    public int XDensity { get; }
    public int YDensity { get; }
    public int ThumbnailWidth { get; }
    public int ThumbnailHeight { get; }
}