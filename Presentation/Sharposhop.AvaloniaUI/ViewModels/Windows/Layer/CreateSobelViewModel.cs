using System.Threading.Tasks;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateSobelViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;
    
    public CreateSobelViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
    }

    public ValueTask Add()
    {
        var layer = new SobelFilter(_enumerationStrategy);
        return _layerManager.Add(layer);
    }
}