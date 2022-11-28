using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImage : IDisposable
{
    int Width { get; }
    int Height { get; }
    
    ColorScheme Scheme { get; }

    ColorTriplet this[int x, int y] { get; }

    event Func<Task> BitmapChanged;
}