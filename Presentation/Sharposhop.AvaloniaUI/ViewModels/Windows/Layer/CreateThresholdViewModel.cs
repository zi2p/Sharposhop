using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateThresholdViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;

    private Fraction _gamma;

    public CreateThresholdViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
    }

    public Fraction Gamma
    {
        get => _gamma;
        set
        {
            _gamma = value;
            this.RaisePropertyChanged();
        }
    }

    public ValueTask Add()
    {
        var layer = new ThresholdFilter(_gamma);
        return _layerManager.Add(layer);
    }
}