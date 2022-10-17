using SkiaSharp;

namespace Sharposhop.Core.Pnm;

public interface IPnmImage
{
    PnmType Type { get; }
    int Height { get; }
    int Width { get; }
    int MaxColor { get; }
    byte[] Bytes { get; }

    SKColor GetColor(int x, int y);
}

public enum PnmType
{
    Gray,
    Colored
}