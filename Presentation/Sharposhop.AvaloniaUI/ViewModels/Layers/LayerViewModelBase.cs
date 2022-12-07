using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public abstract class LayerViewModelBase : ViewModelBase
{
    public abstract ILayer Filter { get; } 
}