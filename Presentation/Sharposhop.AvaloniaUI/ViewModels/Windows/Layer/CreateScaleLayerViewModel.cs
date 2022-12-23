using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Scaling;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

public class CreateScaleLayerViewModel : ViewModelBase
{
    private readonly ILayerManager _layerManager;
    private readonly IEnumerationStrategy _enumerationStrategy;

    private int _width;
    private int _height;
    private ScaleType _type;

    public CreateScaleLayerViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;
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

    public ScaleType Type
    {
        get => _type;
        set => this.RaiseAndSetIfChanged(ref _type, value);
    }

    public IEnumerable<ScaleType> ScaleTypes => Enum.GetValues<ScaleType>();

    public ValueTask Add()
    {
        var size = new PictureSize(_width, _height);

        ILayer layer = _type switch
        {
            ScaleType.Bilinear => new BilinearScalingLayer(_enumerationStrategy, size),
            ScaleType.Lanczos => new Lanczos3ScalingLayer(_enumerationStrategy, size),
            ScaleType.NearestNeighbor => new NearestNeighbourScalingLayer(_enumerationStrategy, size),
            _ => throw new ArgumentOutOfRangeException()
        };

        return _layerManager.Add(layer);
    }
}