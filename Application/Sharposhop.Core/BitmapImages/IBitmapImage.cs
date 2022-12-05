using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImage : IDisposable
{
    int Width { get; }
    int Height { get; }

    ColorScheme Scheme { get; }
    GammaModel Gamma { get; set; }

    ValueTask WriteToAsync<T>(T writer) where T : ITripletWriter;

    event Func<ValueTask> BitmapChanged;
}