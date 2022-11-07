namespace Sharposhop.Core.Model;

public readonly struct Fraction
{
    public Fraction(float value)
    {
        if (value is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");

        Value = value;
    }

    public float Value { get; }

    public static implicit operator float(Fraction fraction)
        => fraction.Value;

    public static implicit operator Fraction(float value)
        => new Fraction(value);
}