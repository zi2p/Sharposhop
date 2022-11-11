using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class HsvSchemeConverter : ISchemeConverter
{
    private readonly IDeNormalizer _deNormalizer;

    public HsvSchemeConverter(IDeNormalizer deNormalizer)
    {
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var max = Math.Max(triplet.First, Math.Max(triplet.Second, triplet.Third));
        var min = Math.Min(triplet.First, Math.Min(triplet.Second, triplet.Third));

        var v = max;
        var s = max is 0 ? 0 : 1 - min / max;
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

        return new ColorTriplet(h, s, v);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var h = triplet.First * 360;
        var s = triplet.Second;
        var v = triplet.Third;

        var c = v * s;
        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        var m = v - c;

        float r;
        float g;
        float b;

        switch (h)
        {
            case < 60:
                (r, g, b) = (c, x, 0);
                break;
            case < 120:
                (r, g, b) = (x, c, 0);
                break;
            case < 180:
                (r, g, b) = (0, c, x);
                break;
            case < 240:
                (r, g, b) = (0, x, c);
                break;
            case < 300:
                (r, g, b) = (x, 0, c);
                break;
            default:
                (r, g, b) = (c, 0, x);
                break;
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