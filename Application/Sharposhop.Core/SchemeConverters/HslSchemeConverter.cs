using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class HslSchemeConverter : ISchemeConverter
{
    SimpleNormalizer _normalizer = new SimpleNormalizer();
    SimpleDeNormalizer _deNormalizer = new SimpleDeNormalizer();
    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var R = _deNormalizer.DeNormalize(triplet.First.Value);
        var G = _deNormalizer.DeNormalize(triplet.Second.Value);
        var B = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        var maxcolor = Math.Max(R, G);
        maxcolor = Math.Max(maxcolor, B);
        var mincolor = Math.Min(R, G);
        mincolor = Math.Min(mincolor, B);
        float hValue, sValue = new float();
        var lValue = (float)((maxcolor + mincolor) / 2);

        if (maxcolor == mincolor)
        {
            hValue = 0;
            sValue = 0;
        }
        else
        {
            if (lValue < 127.5)
            {
                sValue = 255 * (maxcolor - mincolor) / (maxcolor + mincolor);    
            }
            else
            {
                sValue = 255 * (maxcolor - mincolor) / (510 - (maxcolor + mincolor));
            }

            if (R == maxcolor)
            {
                hValue = (G - B) / (maxcolor - mincolor);
            }

            if (G == maxcolor)
            {
                hValue = 120 + (B-R) / (maxcolor - mincolor);
            }
            else
            {
                hValue = 240 + (R-G) / (maxcolor - mincolor);
            }

            if (hValue < 0)
            {
                hValue += 360;
            }

            if (hValue > 360)
            {
                hValue -= 360;
            }
        }

        hValue = _normalizer.Normalize((byte)hValue);
        sValue = _normalizer.Normalize((byte)sValue);
        lValue = _normalizer.Normalize((byte)lValue);

        return new ColorTriplet(new Fraction(hValue), new Fraction(sValue), new Fraction(lValue));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var H = _deNormalizer.DeNormalize(triplet.First.Value);
        var S = _deNormalizer.DeNormalize(triplet.Second.Value);
        var L = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        float rValue, gValue, bValue = new float();
        var temp2 = new float();
        if (S == 0)
        {
            rValue = L;
            gValue = rValue;
            bValue = gValue;
        }
        else
        {
            if (L < 127.5)
            {
                temp2 = L * (255 + S) / 255;
            }
            else
            {
                temp2 = (L+ S) - L * S / 255;
            }

            var temp1 = 2 *L - temp2;
            var temp3 = temp2 - temp1;

            
            // convert R
            var hValue = H + 120;

            if (hValue >= 360)
            {
                hValue -= 360;
            }

            rValue = hValue switch
            {
                < 60 => temp1 + temp3 * hValue / 60,
                < 180 and >= 60 => temp2,
                < 240 and >= 180 => temp1 + temp3 * (4 - hValue / 60),
                _ => temp1
            };
            
            // convert G
            hValue = H;

            gValue = hValue switch
            {
                < 60 => temp1 + temp3 * hValue / 60,
                < 180 and >= 60 => temp2,
                < 240 and >= 180 => temp1 + temp3 * (4 - hValue / 60),
                _ => temp1
            };
            
            // convert B
            hValue = H - 120;

            if (hValue < 0)
            {
                hValue += 360;
            }
            
            bValue = hValue switch
            {
                < 60 => temp1 + temp3 * hValue / 60,
                < 180 and >= 60 => temp2,
                < 240 and >= 180 => temp1 + temp3 * (4 - hValue / 60),
                _ => temp1
            };
        }
        
        rValue = _normalizer.Normalize((byte)rValue);
        gValue = _normalizer.Normalize((byte)gValue);
        bValue = _normalizer.Normalize((byte)bValue);

        return new ColorTriplet(new Fraction(rValue), new Fraction(gValue), new Fraction(bValue));
    }
}