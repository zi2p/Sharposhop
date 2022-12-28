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
    private float _b;
    private float _c;
    private ScaleType _type;

    public CreateScaleLayerViewModel(ILayerManager layerManager, IEnumerationStrategy enumerationStrategy)
    {
        _layerManager = layerManager;
        _enumerationStrategy = enumerationStrategy;

        B = 0;
        C = 0.5f;
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

    public float B
    {
        get => _b;
        set => this.RaiseAndSetIfChanged(ref _b, value);
    }

    public float C
    {
        get => _c;
        set => this.RaiseAndSetIfChanged(ref _c, value);
    }

    public ScaleType Type
    {
        get => _type;
        set
        {
            this.RaiseAndSetIfChanged(ref _type, value);
            this.RaisePropertyChanged(nameof(IsSpline));
        }
    }

    public bool IsSpline => Type is ScaleType.Spline;

    public IEnumerable<ScaleType> ScaleTypes => Enum.GetValues<ScaleType>();

    public ValueTask Add()
    {
        var size = new PictureSize(_width, _height);

        ILayer layer = _type switch
        {
            ScaleType.Bilinear => new BilinearScalingLayer(_enumerationStrategy, size),
            ScaleType.Lanczos => new Lanczos3ScalingLayer(_enumerationStrategy, size),
            ScaleType.NearestNeighbor => new NearestNeighbourScalingLayer(_enumerationStrategy, size),
            ScaleType.Spline => new SplineScalingLayer(_enumerationStrategy, size, _b, _c),
            _ => throw new ArgumentOutOfRangeException(),
        };

        return _layerManager.Add(layer);
    }
}