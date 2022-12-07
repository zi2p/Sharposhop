using System.Runtime.CompilerServices;

namespace Sharposhop.Core.Model;

public readonly struct Gamma
{
    public static readonly Gamma DefaultGamma = 0.0f;

    public Gamma(float value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "GammaModel value can't be less than 0");

        Value = value;
    }

    public float Value { get; }

    public Gamma Reversed => Value == 0 ? 1 / 2.2f : new Gamma(1 / Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator float(Gamma gamma)
        => gamma.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Gamma(float value)
        => new Gamma(value);

    public bool Equals(Gamma other)
        => Value.Equals(other.Value);

    public override bool Equals(object? obj)
        => obj is Gamma other && Equals(other);

    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(Gamma left, Gamma right)
        => left.Equals(right);

    public static bool operator !=(Gamma left, Gamma right)
        => !(left == right);
}