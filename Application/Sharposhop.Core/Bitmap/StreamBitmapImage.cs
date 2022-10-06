namespace Sharposhop.Core.Bitmap;

public class StreamBitmapImage : IBitmapImage
{
    private readonly Stream _stream;

    public StreamBitmapImage(Stream stream, int width)
    {
        _stream = stream;
        Width = width;
    }

    public Stream Stream => GetStreamCopy();

    public int Width { get; }
    public event Action? BitmapChanged;

    private Stream GetStreamCopy()
    {
        var stream = new MemoryStream();

        _stream.CopyTo(stream);
        _stream.Position = 0;
        stream.Position = 0;

        return stream;
    }
}