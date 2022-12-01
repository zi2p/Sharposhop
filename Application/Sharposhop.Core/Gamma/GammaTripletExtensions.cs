using Sharposhop.Core.Model;

namespace Sharposhop.Core.Gamma;

public static class GammaTripletExtensions
{
    public static ColorTriplet WithGamma(this ColorTriplet triplet, GammaModel newGamma)
    {
        if (newGamma == GammaModel.DefaultGamma)
        {
            return new ColorTriplet(
                triplet.First <= 0.040449936 ? triplet.First / 12.92f : (float)Math.Pow((triplet.First + 0.055) / 1.055, 2.4),
                triplet.Second <= 0.040449936 ? triplet.Second / 12.92f : (float)Math.Pow((triplet.Second + 0.055) / 1.055, 2.4),
                triplet.Third <= 0.040449936 ? triplet.Third / 12.92f : (float)Math.Pow((triplet.Third + 0.055) / 1.055, 2.4));
        }

        return new ColorTriplet(
            (float)Math.Pow(triplet.First, newGamma),
            (float)Math.Pow(triplet.Second, newGamma),
            (float)Math.Pow(triplet.Third, newGamma));
    }

    public static ColorTriplet WithoutGamma(this ColorTriplet triplet, GammaModel oldGamma)
    {
        if (oldGamma == GammaModel.DefaultGamma.Reversed)
        {
            return new ColorTriplet(
                triplet.First <= 0.0031308f
                    ? triplet.First * 12.92f
                    : (float) Math.Pow(triplet.First, 1 / 2.4) * 1.055f - 0.055f,
                triplet.Second <= 0.0031308f
                    ? triplet.Second * 12.92f
                    : (float) Math.Pow(triplet.Second, 1 / 2.4) * 1.055f - 0.055f,
                triplet.Third <= 0.0031308f
                    ? triplet.Third * 12.92f
                    : (float) Math.Pow(triplet.Third, 1 / 2.4) * 1.055f - 0.055f);
        }

        return new ColorTriplet(
            (float)Math.Pow(triplet.First, oldGamma.Reversed), 
            (float)Math.Pow(triplet.Second, oldGamma.Reversed),
            (float)Math.Pow(triplet.Third, oldGamma.Reversed));
    }
}