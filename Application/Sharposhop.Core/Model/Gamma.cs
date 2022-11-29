using System.Runtime.CompilerServices;

namespace Sharposhop.Core.Model;

public readonly struct Gamma
{
    public Gamma(float value)
    {
        // TODO: validate value
        Value = value;
    }

    public float Value { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator float(Gamma gamma)
        => gamma.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Gamma(float value)
        => new Gamma(value);
}