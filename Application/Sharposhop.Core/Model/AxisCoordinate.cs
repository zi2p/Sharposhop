using System.Runtime.CompilerServices;

namespace Sharposhop.Core.Model;

public readonly struct AxisCoordinate
{
    public AxisCoordinate(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Coordinate value must be positive.");

        Value = value;
    }

    public int Value { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(AxisCoordinate axisCoordinate)
        => axisCoordinate.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AxisCoordinate(int value)
        => new AxisCoordinate(value);

    public override string ToString()
        => Value.ToString();
}