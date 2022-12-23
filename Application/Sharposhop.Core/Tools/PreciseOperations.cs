using Sharposhop.Core.Model;

namespace Sharposhop.Core.Tools;

public static class PreciseOperations
{
    public static int DeNormalize(double color)
    {
        var value = color * 255;
        return (int)Math.Round(value, 0);
    }

    public static int DeNormalize(Fraction color)
    {
        var value = color * 255;
        return (int)Math.Round(value, 0);
    }

    public static Fraction Normalize(double value) => (float)value / 255;

    public static Fraction Normalize(byte value)  => (float)value / 255;
}