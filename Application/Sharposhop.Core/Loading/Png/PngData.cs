namespace Sharposhop.Core.Loading.Png;

public class PngData
{
    private readonly byte[] _data;
    private readonly int _bytesPerPixel;
    private readonly int _width;
    private readonly Palette? _palette;
    private readonly int _rowOffset;
    private readonly int _bitDepth;

    public PngData(byte[] data, int bytesPerPixel, IhdrData imageHeader, Palette? palette = null)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _bytesPerPixel = bytesPerPixel;
        _palette = palette;
        _width = imageHeader.Width;
        _rowOffset = imageHeader.InterlaceMethod == InterlaceMethod.Adam7 ?
            throw new NotSupportedException("Interlacing is not supported") :
            1;
        _bitDepth = imageHeader.BitDepth;
    }

    public (byte r, byte g, byte b) GetPixel(int x, int y)
    {
        if (_palette is not null)
        {
            var pixelsPerByte = 8 / _bitDepth;
            var bytesInRow = 1 + _width / pixelsPerByte;
            var byteIndexInRow = x / pixelsPerByte;
            var paletteIndex = 1 + y * bytesInRow + byteIndexInRow;
            return _palette.GetColor(_data[paletteIndex]);
        }

        var rowStartPixel = _rowOffset + _rowOffset * y + _bytesPerPixel * _width * y;
        var pixelStartIndex = rowStartPixel + _bytesPerPixel * x;
        var first = _data[pixelStartIndex];

        return _bytesPerPixel switch
        {
            1 => (first, first, first),
            3 => (first, _data[pixelStartIndex + 1], _data[pixelStartIndex + 2]),
            _ => throw new InvalidOperationException($"Unsupported number of bytes per pixel: {_bytesPerPixel}.")
        };
    }
}

public enum PngColor
{
    GrayScale = 0,
    Colored = 2,
    Palette = 3
}

public enum InterlaceMethod
{
    None,
    Adam7
}

public enum FilterMethod
{
    DefaultFiltering
}

public enum CompressionMethod
{
    DefaultCompression
}