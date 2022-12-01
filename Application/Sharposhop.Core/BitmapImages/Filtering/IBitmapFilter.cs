using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilter
{
    string DisplayName { get; }
    UserAction UserAction { get; set; }

    event Func<ValueTask> FilterChanged;

    void Accept(IBitmapFilterVisitor visitor);

    ValueTask WriteAsync<T>(T writer, IBitmapImage image, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator)
        where T : ITripletWriter;
}