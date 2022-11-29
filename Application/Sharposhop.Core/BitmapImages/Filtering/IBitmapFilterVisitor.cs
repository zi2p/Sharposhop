using Sharposhop.Core.BitmapImages.Filtering.Filters;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilterVisitor
{
    void Visit(DimmingBitmapFilter filter);
}