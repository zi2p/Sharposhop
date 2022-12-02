using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.Gamma;

public readonly struct GammaConvertWriter : IBitmapImageWriter
{
    public GammaConvertWriter(GammaModel newGamma, GammaModel oldGamma)
    {
        NewGamma = newGamma;
        OldGamma = oldGamma;
    }

    public GammaModel NewGamma { get; }
    public GammaModel OldGamma { get; }

    public ValueTask<ColorTriplet> Write(PlaneCoordinate coordinate, ColorTriplet current)
        => ValueTask.FromResult(current.WithoutGamma(OldGamma).WithGamma(NewGamma));
}