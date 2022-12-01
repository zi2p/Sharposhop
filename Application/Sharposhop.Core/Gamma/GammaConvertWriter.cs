using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.Gamma;

public class GammaConvertWriter : IBitmapImageWriter
{
    public GammaConvertWriter(GammaModel newGamma, GammaModel oldGamma)
    {
        NewGamma = newGamma;
        OldGamma = oldGamma;
    }

    public GammaModel NewGamma { get; }
    public GammaModel OldGamma { get; }

    public ValueTask<ColorTriplet> Write(PlaneCoordinate coordinate, ColorTriplet current)
    {
        return ValueTask.FromResult(current.WithGamma(OldGamma).WithoutGamma(NewGamma));
    }
}