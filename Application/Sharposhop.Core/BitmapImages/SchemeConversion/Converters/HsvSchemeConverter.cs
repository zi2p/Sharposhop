using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.SchemeConversion.Converters;

public class HsvSchemeConverter : ISchemeConverter
{
    private const float Delta = 0.01f;

    public ColorScheme Scheme => ColorScheme.Hsv;

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var max = Math.Max(triplet.First, Math.Max(triplet.Second, triplet.Third));
        var min = Math.Min(triplet.First, Math.Min(triplet.Second, triplet.Third));

        var v = max;
        var s = max is 0 ? 0 : 1 - min / max;
        float h = 0;

        if (Math.Abs(max - min) < Delta)
        {
            h = 0;
        }
        else if (Math.Abs(max - triplet.First) < Delta && triplet.Second >= triplet.Third)
        {
            h = 60 * (triplet.Second - triplet.Third) / (max - min);
        }
        else if (Math.Abs(max - triplet.First) < Delta && triplet.Second < triplet.Third)
        {
            h = 60 * (triplet.Second - triplet.Third) / (max - min) + 360;
        }
        else if (Math.Abs(max - triplet.Second) < Delta)
        {
            h = 60 * (triplet.Third - triplet.First) / (max - min) + 120;
        }
        else if (Math.Abs(max - triplet.Third) < Delta)
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

        var (r, g, b) = h switch
        {
            < 60 => (c, x, 0f),
            < 120 => (x, c, 0f),
            < 180 => (0f, c, x),
            < 240 => (0f, x, c),
            < 300 => (x, 0f, c),
            _ => (c, 0f, x)
        };

        return new ColorTriplet(r + m, g + m, b + m);
    }
}