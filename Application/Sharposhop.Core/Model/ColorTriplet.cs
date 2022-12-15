namespace Sharposhop.Core.Model;

public readonly record struct ColorTriplet(Fraction First, Fraction Second, Fraction Third)
{
    public float Average => (First + Second + Third) / 3;
}