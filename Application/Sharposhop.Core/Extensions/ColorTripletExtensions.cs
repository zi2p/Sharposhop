using Sharposhop.Core.Model;

namespace Sharposhop.Core.Extensions;

public static class ColorTripletExtensions
{
    public static ColorTriplet WithGamma(this ColorTriplet triplet, Gamma newGamma)
    {
        if (newGamma == Gamma.DefaultGamma)
        {
            return new ColorTriplet(
                triplet.First <= 0.0404482362771082f ? triplet.First / 12.92f : (float)Math.Pow((triplet.First + 0.055) / 1.055, 2.4),
                triplet.Second <= 0.0404482362771082f ? triplet.Second / 12.92f : (float)Math.Pow((triplet.Second + 0.055) / 1.055, 2.4),
                triplet.Third <= 0.0404482362771082f ? triplet.Third / 12.92f : (float)Math.Pow((triplet.Third + 0.055) / 1.055, 2.4));
        }

        return new ColorTriplet(
            (float)Math.Pow(triplet.First, newGamma),
            (float)Math.Pow(triplet.Second, newGamma),
            (float)Math.Pow(triplet.Third, newGamma));
    }

    public static ColorTriplet WithoutGamma(this ColorTriplet triplet, Gamma oldGamma)
    {
        if (oldGamma == Gamma.DefaultGamma)
        {
            return new ColorTriplet(
                triplet.First <= 0.00313066844250063f
                    ? triplet.First * 12.92f
                    : (float)(Math.Pow(triplet.First, 1 / 2.4) * 1.055 - 0.055),
                triplet.Second <= 0.00313066844250063f
                    ? triplet.Second * 12.92f
                    : (float)(Math.Pow(triplet.Second, 1 / 2.4) * 1.055 - 0.055),
                triplet.Third <= 0.00313066844250063f
                    ? triplet.Third * 12.92f
                    : (float)(Math.Pow(triplet.Third, 1 / 2.4) * 1.055 - 0.055));
        }

        return new ColorTriplet(
            (float)Math.Pow(triplet.First, oldGamma.Reversed), 
            (float)Math.Pow(triplet.Second, oldGamma.Reversed),
            (float)Math.Pow(triplet.Third, oldGamma.Reversed));
    }
}