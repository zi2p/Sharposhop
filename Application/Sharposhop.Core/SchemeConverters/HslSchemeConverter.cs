using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class HslSchemeConverter : ISchemeConverter
{
    private readonly IDeNormalizer _deNormalizer;

    public HslSchemeConverter(IDeNormalizer deNormalizer)
    {
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var max = Math.Max(triplet.First, Math.Max(triplet.Second, triplet.Third));
        var min = Math.Min(triplet.First, Math.Min(triplet.Second, triplet.Third));

        var l = (max + min) / 2;
        var s = (max - min) / (1 - Math.Abs(1 - (max + min)));

        float h = 0;

        if (max == min)
        {
            h = 0;
        }
        else if (max == triplet.First && triplet.Second >= triplet.Third)
        {
            h = 60 * (triplet.Second - triplet.Third) / (max - min);
        }
        else if (max == triplet.First && triplet.Second < triplet.Third)
        {
            h = 60 * (triplet.Second - triplet.Third) / (max - min) + 360;
        }
        else if (max == triplet.Second)
        {
            h = 60 * (triplet.Third - triplet.First) / (max - min) + 120;
        }
        else if (max == triplet.Third)
        {
            h = 60 * (triplet.First - triplet.Second) / (max - min) + 240;
        }

        h /= 360;

        return new ColorTriplet(h, s, l);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var c = (1 - Math.Abs(2 * triplet.Third - 1)) * triplet.Second;
        var x = c * (1 - Math.Abs((triplet.First * 6) % 2 - 1));
        var m = triplet.Third - c / 2;

        float r = 0, g = 0, b = 0;

        if (triplet.First < 1f / 6)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (triplet.First < 2f / 6)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (triplet.First < 3f / 6)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (triplet.First < 4f / 6)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (triplet.First < 5f / 6)
        {
            r = x;
            g = 0;
            b = c;
        }
        else if (triplet.First < 6f / 6)
        {
            r = c;
            g = 0;
            b = x;
        }

        return new ColorTriplet(r + m, g + m, b + m);
    }

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        return (
            _deNormalizer.DeNormalize(triplet.First),
            _deNormalizer.DeNormalize(triplet.Second),
            _deNormalizer.DeNormalize(triplet.Third));
    }
}