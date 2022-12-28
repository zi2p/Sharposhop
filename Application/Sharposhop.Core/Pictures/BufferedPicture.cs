using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public sealed class BufferedPicture : Picture
{
    private readonly BufferPicture _bufferPicture;

    public BufferedPicture(
        PictureSize size,
        ColorScheme scheme,
        Gamma gamma,
        DisposableArray<ColorTriplet> layer)
        : base(size, scheme, gamma, layer)
    {
        _bufferPicture = new BufferPicture(this);
    }

    public IPicture GetBufferPicture()
    {
        _bufferPicture.Reload();
        return _bufferPicture;
    }

    public override void Dispose()
    {
        base.Dispose();
        _bufferPicture.Dispose();
    }
}