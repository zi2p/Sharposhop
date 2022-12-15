namespace Sharposhop.Core.Loading.Png;

public readonly struct IhdrData
{
    public int Width { get; }
    public int Height { get; }
    public byte BitDepth { get; }
    public PngColor Color { get; }
    public CompressionMethod CompressionMethod { get; }
    public FilterMethod FilterMethod { get; }
    public InterlaceMethod InterlaceMethod { get; }


    public IhdrData(int width, int height, byte bitDepth, PngColor color,
        CompressionMethod compressionMethod, FilterMethod filterMethod, InterlaceMethod interlaceMethod)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), $"{width}px is invalid width of image.");

        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), $"{height}px is invalid height of image.");

        if (bitDepth != 8)
            throw new NotSupportedException("Only 8 bits per channel supported");

        Width = width;
        Height = height;
        BitDepth = bitDepth;
        Color = color;
        CompressionMethod = compressionMethod;
        FilterMethod = filterMethod;
        InterlaceMethod = interlaceMethod;
    }
}