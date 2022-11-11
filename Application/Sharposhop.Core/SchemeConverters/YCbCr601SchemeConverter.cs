using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCbCr601SchemeConverter : ISchemeConverter
{
    SimpleNormalizer _normalizer = new SimpleNormalizer();
    SimpleDeNormalizer _deNormalizer = new SimpleDeNormalizer();
    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var R = _deNormalizer.DeNormalize(triplet.First.Value);
        var G = _deNormalizer.DeNormalize(triplet.Second.Value);
        var B = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        float yValue, cbValue, crValue = new float();
        yValue = (float) (0.257 * R + 0.504 * G + 0.098 * B + 16);
        cbValue = (float) (-0.148 * R - 0.291 * G + 0.439 * B + 128);
        crValue = (float) (0.439 * R - 0.368 * G - 0.071 * B + 128);
        
        yValue = _normalizer.Normalize((byte)yValue);
        cbValue = _normalizer.Normalize((byte)cbValue);
        crValue = _normalizer.Normalize((byte)crValue);
        
        return new ColorTriplet(new Fraction(yValue), new Fraction(cbValue), new Fraction(crValue));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var Y = _deNormalizer.DeNormalize(triplet.First.Value);
        var Cb = _deNormalizer.DeNormalize(triplet.Second.Value);
        var Cr = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        float rValue, gValue, bValue = new float();
        rValue = (float) (1.164 * (Y - 16) + 1.596 * (Cr - 128));
        gValue = (float) (1.164 * (Y - 16) - 0.392 * (Cb - 128) - 0.813 * (Cr - 128));
        bValue = (float) (1.164 * (Y - 16 + 2.017 * (Cb - 128)));

        rValue = _normalizer.Normalize((byte)rValue);
        gValue = _normalizer.Normalize((byte)gValue);
        bValue = _normalizer.Normalize((byte)bValue);
        
        return new ColorTriplet(new Fraction(rValue), new Fraction(gValue), new Fraction(bValue));
    }
}