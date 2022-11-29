using Sharposhop.Core.BitmapImages.Filtering;

namespace Sharposhop.AvaloniaUI.FilterProvider;

public interface IFilterProvider
{
    string DisplayName { get; }

    IBitmapFilter Create();
}