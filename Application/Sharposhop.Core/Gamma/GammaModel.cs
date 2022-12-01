using System.Runtime.CompilerServices;

namespace Sharposhop.Core.Gamma;

public readonly struct GammaModel
{
    public static readonly GammaModel DefaultGamma = 0.0f;

    public GammaModel(float value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "GammaModel value can't be less than 0");
        Value = value;
    }

    public float Value { get; }

    public GammaModel Reversed => Value == 0 ? 1 / 2.2f : new GammaModel(1 / Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator float(GammaModel gamma)
        => gamma.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator GammaModel(float value)
        => new GammaModel(value);
}