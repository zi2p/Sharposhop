using ReactiveUI;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Dithering;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateDitheringLayerViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly Func<int, IDitheringLayer> _layerFactory;

    private int _depth;

    public CreateDitheringLayerViewModel(
        string name,
        ILayerManager layerManager,
        Func<int, IDitheringLayer> layerFactory)
    {
        _layerManager = layerManager;
        _layerFactory = layerFactory;
        Name = name;

        _depth = 1;
    }
    
    public string Name { get; }

    public int Depth
    {
        get => _depth;
        set => this.RaiseAndSetIfChanged(ref _depth, value);
    }

    public ValueTask AddAsync()
    {
        var layer = _layerFactory.Invoke(_depth);
        return _layerManager.Add(layer);
    }
}