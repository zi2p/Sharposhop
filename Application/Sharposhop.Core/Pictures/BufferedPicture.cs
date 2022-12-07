using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public sealed class BufferedPicture : Picture, IDisposable
{
    private readonly BufferPicture _bufferPicture;

    public BufferedPicture(
        PictureSize size,
        ColorScheme scheme,
        Gamma gamma,
        IEnumerationStrategy enumerationStrategy,
        ColorTriplet[] layer)
        : base(size, scheme, gamma, enumerationStrategy, layer)
    {
        _bufferPicture = new BufferPicture(this, enumerationStrategy);
    }

    public IPicture GetBufferPicture()
    {
        _bufferPicture.Reload();
        return _bufferPicture;
    }

    public void Dispose()
        => _bufferPicture.Dispose();
}