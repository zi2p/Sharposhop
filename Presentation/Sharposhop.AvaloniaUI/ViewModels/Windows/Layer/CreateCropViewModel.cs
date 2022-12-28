using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateCropViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;

    private int _x;
    private int _y;
    private int _width;
    private int _height;

    public CreateCropViewModel(
        ILayerManager layerManager,
        IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
    }

    public int X
    {
        get => _x;
        set => this.RaiseAndSetIfChanged(ref _x, value);
    }

    public int Y
    {
        get => _y;
        set => this.RaiseAndSetIfChanged(ref _y, value);
    }

    public int Width
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    public int Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    public ValueTask Add()
    {
        var anchor = new PlaneCoordinate(_x, _y);
        var size = new PictureSize(_width, _height);
        var layer = new CropLayer(_enumerationStrategy, anchor, size);

        return _layerManager.Add(layer);
    }
}