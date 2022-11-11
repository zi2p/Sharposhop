using MathNet.Numerics.LinearAlgebra.Single;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCoCgSchemeConverter : ISchemeConverter
{
    private readonly IDeNormalizer _deNormalizer;

    public YCoCgSchemeConverter(IDeNormalizer deNormalizer)
    {
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var matrix = new DenseMatrix(3, 3, new[]
        {
            1 / 4f, 1 / 2f, 1 / 4f,
            1 / 2f, 0, -1 / 2f,
            -1 / 4f, 1 / 2f, -1 / 4f,
        });

        var vector = new DenseVector(new float[] { triplet.First, triplet.Second, triplet.Third });

        var hsl = matrix.Transpose() * vector;

        return new ColorTriplet(hsl[0], hsl[1] + 0.5f, hsl[2] + 0.5f);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = triplet.First.Value;
        var co = triplet.Second - 0.5f;
        var cg = triplet.Third - 0.5f;

        var tmp = y - cg;

        var r = tmp + co;
        var g = y + cg;
        var b = tmp - co;

        return new ColorTriplet(r, g, b);
    }

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        return (
            _deNormalizer.DeNormalize(triplet.First),
            _deNormalizer.DeNormalize(triplet.Second),
            _deNormalizer.DeNormalize(triplet.Third));
    }
}