using SkiaSharp;

namespace Sharposhop.Core.Pnm;

public class GrayPnmImage : IPnmImage
{
    public GrayPnmImage(int height, int width, int maxColor, byte[] bytes)
    {
        Height = height;
        Width = width;
        MaxColor = maxColor;
        Bytes = bytes;
    }

    public PnmType Type => PnmType.Gray;
    public int Height { get; }
    public int Width { get; }
    public int MaxColor { get; }
    public byte[] Bytes { get; }

    public SKColor GetColor(int x, int y)
    {
        var color = Bytes[y * Width + x];
        return new SKColor(color, color, color, 255);
    }
}