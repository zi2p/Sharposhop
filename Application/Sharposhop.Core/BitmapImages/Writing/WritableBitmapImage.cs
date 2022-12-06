using System.Buffers;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Writing;

public sealed class WritableBitmapImage : IBitmapImage
{
    private readonly IReadBitmapImage _bitmap;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private bool _hasWritten;
    private ColorTriplet?[] _layer;

    public WritableBitmapImage(IReadBitmapImage bitmap, IEnumerationStrategy enumerationStrategy)
    {
        _bitmap = bitmap;
        _enumerationStrategy = enumerationStrategy;

        bitmap.BitmapChanged += OnBitmapChanged;

        _layer = ArrayPool<ColorTriplet?>.Shared.Rent(bitmap.Width * bitmap.Height);
        Gamma = bitmap.Gamma;
        _hasWritten = false;
    }

    public int Width => _bitmap.Width;
    public int Height => _bitmap.Height;

    public ColorTriplet this[PlaneCoordinate coordinate]
    {
        get
        {
            if (_hasWritten is false)
                return _bitmap[coordinate];

            var index = _enumerationStrategy.AsContinuousIndex(coordinate, Width, Height);
            ColorTriplet? modificationTriplet = _layer[index];

            return modificationTriplet ?? _bitmap[coordinate];
        }
    }

    public ColorScheme Scheme => _bitmap.Scheme;

    public GammaModel Gamma { get; private set; }

    public event Func<ValueTask>? BitmapChanged;

    public ValueTask UpdateGamma(GammaModel gamma, bool notify)
    {
        if (Gamma.Equals(gamma))
            return ValueTask.CompletedTask;
        
        foreach (var coordinate in _enumerationStrategy.Enumerate(Width, Height))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, Width, Height);

            var triplet = _layer[index] ?? _bitmap[coordinate];
            triplet = triplet.WithGamma(Gamma).WithoutGamma(gamma);

            _layer[index] = triplet;
        }

        Gamma = gamma;

        return BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;
    }

    public ValueTask WriteFromAsync(IEnumerable<PositionedColorTriplet> data, bool notify = true)
    {
        foreach (var (coordinate, triplet) in data)
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, Width, Height);
            _layer[index] = triplet;
        }

        _hasWritten = true;
        return notify ? OnBitmapChanged() : ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _bitmap.BitmapChanged -= OnBitmapChanged;
        _bitmap.Dispose();
        _hasWritten = false;
    }

    private async ValueTask OnBitmapChanged()
    {
        await (BitmapChanged?.Invoke() ?? ValueTask.CompletedTask);

        ArrayPool<ColorTriplet?>.Shared.Return(_layer);
        _layer = ArrayPool<ColorTriplet?>.Shared.Rent(_bitmap.Width * _bitmap.Height);
        _hasWritten = false;
    }
}