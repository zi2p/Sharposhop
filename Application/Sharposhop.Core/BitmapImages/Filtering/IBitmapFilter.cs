using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilter
{
    string DisplayName { get; }

    event Func<ValueTask> FilterChanged;

    void Accept(IBitmapFilterVisitor visitor);

    ValueTask WriteAsync<T>(T writer, IBitmapImage image, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator)
        where T : ITripletWriter;
}