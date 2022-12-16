using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateGaussianViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;

    private int _sigma;

    public CreateGaussianViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
    }

    public int Sigma
    {
        get => _sigma;
        set
        {
            _sigma = value;
            ((IReactiveObject)this).RaisePropertyChanged();
        }
    }

    public ValueTask Add()
    {
        var layer = new GaussianFilter(_sigma, _enumerationStrategy);
        return _layerManager.Add(layer);
    }
}