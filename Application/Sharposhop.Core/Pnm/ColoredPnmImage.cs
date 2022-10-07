using SkiaSharp;

namespace Sharposhop.Core.Pnm;

public class ColoredPnmImage : IPnmImage
{
    public ColoredPnmImage(int height, int width, int maxColor, byte[] bytes)
    {
        Height = height;
        Width = width;
        MaxColor = maxColor;
        Bytes = bytes;
    }

    public PnmType Type => PnmType.Colored;
    public int Height { get; }
    public int Width { get; }
    public int MaxColor { get; }
    public byte[] Bytes { get; }

    public SKColor GetColor(int x, int y)
    {
        y *= 3;
        x *= 3;
        var blue = Bytes[y * Width + x];
        var green = Bytes[y * Width + x + 1];
        var red = Bytes[y * Width + x + 2];

        return new SKColor(red, green, blue, 255);
    }
}