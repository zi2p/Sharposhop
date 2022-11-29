using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilterManager
{
    IEnumerable<IBitmapFilter> Filters { get; }

    void Add(int index, IBitmapFilter filter);
    void Remove(IBitmapFilter filter);

    void Promote(IBitmapFilter filter);
    void Demote(IBitmapFilter filter);
}