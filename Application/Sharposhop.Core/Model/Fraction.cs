using System.Globalization;
using System.Runtime.CompilerServices;

namespace Sharposhop.Core.Model;

public readonly struct Fraction
{
    public Fraction(float value)
    {
        Value = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value,
        };
    }

    public float Value { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator float(Fraction fraction)
        => fraction.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Fraction(float value)
        => new Fraction(value);

    public override string ToString()
        => Value.ToString(CultureInfo.InvariantCulture);
}