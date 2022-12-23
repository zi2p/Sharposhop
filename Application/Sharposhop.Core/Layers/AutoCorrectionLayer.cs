using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;
using Sharposhop.Core.Tools;

namespace Sharposhop.Core.Layers;

public class AutoCorrectionLayer : ILayer
{
    private readonly int _minCoefficient;
    private readonly int _maxCoefficient;

    public AutoCorrectionLayer(int minCoefficient, int maxCoefficient)
    {
        _minCoefficient = minCoefficient;
        _maxCoefficient = maxCoefficient;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        var triplets = picture.AsSpan();
        for (var i = 0; i < triplets.Length; i++)
            triplets[i] = ApplyCorrection(triplets[i]);

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    private ColorTriplet ApplyCorrection(ColorTriplet triplet)
    {
        var (first, second, third) = triplet;
        var r = (PreciseOperations.DeNormalize(first) - _minCoefficient) * 255.0 / (_maxCoefficient - _minCoefficient);
        var g = (PreciseOperations.DeNormalize(second) - _minCoefficient) * 255.0 / (_maxCoefficient - _minCoefficient);
        var b = (PreciseOperations.DeNormalize(third) - _minCoefficient) * 255.0 / (_maxCoefficient - _minCoefficient);

        return new ColorTriplet(
            PreciseOperations.Normalize(r),
            PreciseOperations.Normalize(g),
            PreciseOperations.Normalize(b));
    }
}