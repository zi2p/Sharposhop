using System.Threading.Tasks;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;
using Sharposhop.Core.Normalization;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateOtsuViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    public CreateOtsuViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy, INormalizer normalizer)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
        _normalizer = normalizer;
    }

    public ValueTask Add()
    {
        var layer = new OtsuFilter(_normalizer);
        return _layerManager.Add(layer);
    }
}