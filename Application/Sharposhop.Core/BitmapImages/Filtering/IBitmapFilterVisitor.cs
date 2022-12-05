namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilterVisitor
{
    void Visit(IBitmapFilter filter);
}