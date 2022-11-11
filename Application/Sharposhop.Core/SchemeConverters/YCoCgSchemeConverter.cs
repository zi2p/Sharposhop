using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCoCgSchemeConverter : ISchemeConverter
{
    SimpleNormalizer _normalizer = new SimpleNormalizer();
    SimpleDeNormalizer _deNormalizer = new SimpleDeNormalizer();
    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var R = _deNormalizer.DeNormalize(triplet.First.Value);
        var G = _deNormalizer.DeNormalize(triplet.Second.Value);
        var B = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        float yValue, coValue, cgValue = new float();
        yValue = R / 4 + G / 2 + B / 4;
        coValue = R / 2 - B / 2;
        cgValue = -R / 4 + G / 2 - B / 4;
        
        yValue = _normalizer.Normalize((byte)yValue);
        coValue = _normalizer.Normalize((byte)coValue);
        cgValue = _normalizer.Normalize((byte)cgValue);
        
        return new ColorTriplet(new Fraction(yValue), new Fraction(coValue), new Fraction(cgValue));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var Y = _deNormalizer.DeNormalize(triplet.First.Value);
        var Co = _deNormalizer.DeNormalize(triplet.Second.Value);
        var Cg = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        float rValue, gValue, bValue = new float();
        rValue = Y + Co - Cg;
        gValue = Y + Cg;
        bValue = Y - Co - Cg;
        
        rValue = _normalizer.Normalize((byte)rValue);
        gValue = _normalizer.Normalize((byte)gValue);
        bValue = _normalizer.Normalize((byte)bValue);

        return new ColorTriplet(new Fraction(rValue), new Fraction(gValue), new Fraction(bValue));
    }
}