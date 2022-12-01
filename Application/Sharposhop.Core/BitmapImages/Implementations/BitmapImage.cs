using System.Buffers;
using System.Runtime.CompilerServices;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Implementations;

public sealed class BitmapImage : IWritableBitmapImage
{
    private readonly ColorTriplet[] _values;
    private readonly IEnumerationStrategy _enumeration;
    private GammaModel _gamma;

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
        _gamma = gamma;
        Scheme = scheme;
    }

    public int Width { get; }
    public int Height { get; }
    public ColorScheme Scheme { get; }

    public GammaModel Gamma
    {
        get => _gamma;
        set =>_gamma = value;
    }

    public event Func<ValueTask>? BitmapChanged;

    public async ValueTask WriteToAsync<T>(T writer) where T : ITripletWriter
    {
        foreach (var coordinate in _enumeration.Enumerate(Width, Height))
        {
            var index = _enumeration.AsContinuousIndex(coordinate, Width, Height);
            await writer.WriteAsync(coordinate, _values[index]);
        }
    }

    public async ValueTask WriteFromAsync<T>(IEnumerable<PlaneCoordinate> coordinates, T writer)
        where T : IBitmapImageWriter
    {
        foreach (var coordinate in coordinates)
        {
            ValidateBounds(coordinate);
            var index = _enumeration.AsContinuousIndex(coordinate, Width, Height);
            _values[index] = await writer.Write(coordinate, _values[index]);
        }

        await OnBitmapChanged();
    }

    public void Dispose()
        => ArrayPool<ColorTriplet>.Shared.Return(_values);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void ValidateBounds(PlaneCoordinate coordinate)
    {
        if (coordinate.X >= Width || coordinate.Y >= Height)
            throw new ArgumentOutOfRangeException(nameof(coordinate), "Coordinate is out of bounds.");
    }

    private ValueTask OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;
}