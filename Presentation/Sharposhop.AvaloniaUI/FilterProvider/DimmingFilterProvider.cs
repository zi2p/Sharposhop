using Sharposhop.Core.BitmapImages.Filtering;
using Sharposhop.Core.BitmapImages.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.FilterProvider;

public class DimmingFilterProvider : IFilterProvider
{
    public string DisplayName => "Dimming";

    public IBitmapFilter Create()
        => new DimmingBitmapFilter();
}