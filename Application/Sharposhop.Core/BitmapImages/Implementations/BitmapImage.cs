using System.Buffers;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Implementations;

public sealed class BitmapImage : IReadBitmapImage
{
    private readonly ColorTriplet[] _values;
    private readonly IEnumerationStrategy _enumeration;

    public BitmapImage(
        int width,
        int height,
        ColorScheme scheme,
        GammaModel gamma,
        ColorTriplet[] values,
        IEnumerationStrategy enumeration)
    {
        Width = width;
        Height = height;
        _values = values;
        _enumeration = enumeration;
        Gamma = gamma;
        Scheme = scheme;
    }

    public int Width { get; }
    public int Height { get; }

    public ColorTriplet this[PlaneCoordinate coordinate]
        => _values[_enumeration.AsContinuousIndex(coordinate, Width, Height)];

    public ColorScheme Scheme { get; }
    public GammaModel Gamma { get; }

    public event Func<ValueTask>? BitmapChanged;

    public void Dispose()
        => ArrayPool<ColorTriplet>.Shared.Return(_values);
}