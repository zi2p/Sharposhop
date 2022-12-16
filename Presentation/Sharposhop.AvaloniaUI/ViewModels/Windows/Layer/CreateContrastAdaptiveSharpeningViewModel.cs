using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateContrastAdaptiveSharpeningViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;

    private float _sharpness;

    public CreateContrastAdaptiveSharpeningViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
    }

    public float Sharpness
    {
        get => _sharpness;
        set
        {
            _sharpness= value;
            this.RaisePropertyChanged();
        }
    }

    public ValueTask Add()
    {
        var layer = new ContrastAdaptiveSharpening(_sharpness, _enumerationStrategy);
        return _layerManager.Add(layer);
    }
}