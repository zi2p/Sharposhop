using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class HsvSchemeConverter : ISchemeConverter
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
        float hValue, sValue, vValue = new float();

        vValue = maxcolor;
        hValue = 0;

        if (maxcolor == mincolor)
        {
            hValue = 0;
        }
        
        else if (maxcolor == R && G >= B)
        {
            hValue = 60 * (G - B) / (maxcolor - mincolor);
        }
        
        else if (maxcolor == R && G < B)
        {
            hValue = 60 * (G - B) / (maxcolor - mincolor) + 360;
        }
        
        else if (maxcolor == G)
        {
            hValue = 60 * (B - R) / (maxcolor - mincolor) + 120;
        }
        
        else if (maxcolor == B)
        {
            hValue = 60 * (R - G) / (maxcolor - mincolor) + 240;
        }

        if (maxcolor == 0)
        {
            sValue = 0;
        }
        else
        {
            sValue = (maxcolor - mincolor) / maxcolor;
        }

        hValue = _normalizer.Normalize((byte)hValue);
        sValue = _normalizer.Normalize((byte)sValue);
        vValue = _normalizer.Normalize((byte)vValue);

        return new ColorTriplet(new Fraction(hValue), new Fraction(sValue), new Fraction(vValue));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var H = _deNormalizer.DeNormalize(triplet.First.Value);
        var S = _deNormalizer.DeNormalize(triplet.Second.Value);
        var V = _deNormalizer.DeNormalize(triplet.Third.Value);
        
        float rValue, gValue, bValue = new float();
        var i = 0;
        var hValue = H;

        if (S / 100 == 0)
        {
            rValue = V / 100;
            gValue = rValue;
            bValue = gValue;
        }
        else 
        {
            hValue = (byte) (hValue / 60);
            i = hValue;
            var temp1 = hValue - i;
            float temp2, temp3, temp4 = new float();
 
            temp2 = V / 100 * (1 - S / 100);
            temp3 = V / 100 * (1 - S / 100 * temp1);
            temp4 = V / 100 * (1 - S / 100 * (1 - temp1));
            
            switch(i)
            {
                case 0: 
                    rValue = V / 100; 
                    gValue = temp4; 
                    bValue = temp2; 
                    break;
                case 1: 
                    rValue = temp3; 
                    gValue = V / 100; 
                    bValue = temp2; 
                    break;
                case 2: 
                    rValue = temp2; 
                    gValue = V / 100; 
                    bValue = temp4; 
                    break;
                case 3: 
                    rValue = temp2; 
                    gValue = temp3; 
                    bValue = V / 100; 
                    break;
                case 4: 
                    rValue = temp4; 
                    gValue = temp2; 
                    bValue = V / 100; 
                    break;
                default: 
                    rValue = V / 100; 
                    gValue = temp2; 
                    bValue = temp3; 
                    break;
            }
        }
        rValue *= 100;
        gValue *= 100;
        bValue *= 100;

        rValue = _normalizer.Normalize((byte)rValue);
        gValue = _normalizer.Normalize((byte)gValue);
        bValue = _normalizer.Normalize((byte)bValue);
        
        return new ColorTriplet(new Fraction(rValue), new Fraction(gValue), new Fraction(bValue));
    }
}