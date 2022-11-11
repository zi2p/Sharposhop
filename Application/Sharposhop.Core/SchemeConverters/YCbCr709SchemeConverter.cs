using MathNet.Numerics.LinearAlgebra.Single;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCbCr709SchemeConverter : ISchemeConverter
{
    SimpleNormalizer _normalizer = new SimpleNormalizer();
    SimpleDeNormalizer _deNormalizer = new SimpleDeNormalizer();
    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var r = _deNormalizer.DeNormalize(triplet.First);
        var g = _deNormalizer.DeNormalize(triplet.Second);
        var b = _deNormalizer.DeNormalize(triplet.Third);

        var matrix = new DenseMatrix(3, 3, new[]
        {
            0.183f, 0.614f, 0.062f,
            -0.101f, -0.338f, 0.439f,
            0.439f, -0.399f, -0.040f,
        }).Transpose();

        var vector = new DenseVector(new float[] { r, g, b });

        var left = new DenseVector(new float[] { 16f, 128f, 128f });

        var result = left + matrix * vector;

        var denominator = 240 - 16f;

        return new ColorTriplet(
            (result[0] - 16) / (235 - 16),
            (result[1] - 16) / denominator,
            (result[2] - 16) / denominator);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = triplet.First * (235 - 16) + 16;
        var cb = triplet.Second * (240 - 16) + 16;
        var cr = triplet.Third * (240 - 16) + 16;

        var matrix = new DenseMatrix(3, 3, new[]
        {
            1.164f, 0.000f, 1.793f,
            1.164f, -0.213f, -0.533f,
            1.164f, 2.112f, 0.000f,
        }).Transpose();

        var vector = new DenseVector(new[] { y - 16, cb - 128, cr - 128 });

        var result = matrix * vector;

        return new ColorTriplet(
            _normalizer.Normalize((byte)result[0]),
            _normalizer.Normalize((byte)result[1]),
            _normalizer.Normalize((byte)result[2]));
    }
    

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        var y = (byte)(triplet.First * (235 - 16) + 16);
        var cb = (byte)(triplet.Second * (240 - 16) + 16);
        var cr = (byte)(triplet.Third * (240 - 16) + 16);
        
        return (y, cb, cr);
    }
}