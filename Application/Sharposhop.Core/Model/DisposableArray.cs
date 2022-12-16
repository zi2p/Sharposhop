using System.Buffers;

namespace Sharposhop.Core.Model;

public readonly struct DisposableArray<T> : IDisposable
{
    private readonly T[] _array;

    private DisposableArray(T[] array, int size)
    {
        _array = array;
        Length = size;
    }

    public int Length { get; }

    public static DisposableArray<T> OfSize(int size)
    {
        T[] array = ArrayPool<T>.Shared.Rent(size);
        return new DisposableArray<T>(array, size);
    }

    public Span<T> AsSpan() 
        => _array.AsSpan(0, Length);

    public void Dispose()
        => ArrayPool<T>.Shared.Return(_array);
}