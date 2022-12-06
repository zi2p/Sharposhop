using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImage : IReadBitmapImage
{
    ValueTask WriteFromAsync(IEnumerable<PositionedColorTriplet> data, bool notify = true);
    
    ValueTask UpdateGamma(GammaModel gamma, bool notify = true);
}