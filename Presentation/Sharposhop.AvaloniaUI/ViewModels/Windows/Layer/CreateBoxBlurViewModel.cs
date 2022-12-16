using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateBoxBlurViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;

    private int _radius;

    public CreateBoxBlurViewModel(
        ILayerManager layerManager,
        IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
    }

    public int Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            this.RaisePropertyChanged();
        }
    }

    public ValueTask Add()
    {
        var layer = new BoxBlurFilter(_radius, _enumerationStrategy);
        return _layerManager.Add(layer);
    }
}