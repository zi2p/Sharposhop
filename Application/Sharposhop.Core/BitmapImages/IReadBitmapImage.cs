using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public interface IReadBitmapImage : IDisposable
{
    int Width { get; }
    int Height { get; }

    ColorTriplet this[PlaneCoordinate coordinate] { get; }

    ColorScheme Scheme { get; }
    GammaModel Gamma { get; }

    event Func<ValueTask> BitmapChanged;
}