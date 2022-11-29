using Sharposhop.Core.BitmapImages.Filtering;

namespace Sharposhop.AvaloniaUI.ViewModels.Filters;

public abstract class FilterViewModelBase : ViewModelBase
{
    public abstract IBitmapFilter Filter { get; } 
}