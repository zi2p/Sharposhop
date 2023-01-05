using System.Collections.Generic;
using Sharposhop.AvaloniaUI.ViewModels.Layers;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Dithering;
using Sharposhop.Core.Layers.Filtering.Filters;
using Sharposhop.Core.Layers.Scaling;

namespace Sharposhop.AvaloniaUI.Tools;

public class BitmapFilterViewVisitor : ILayerVisitor
{
    private readonly ILayerManager _layerManager;

    public BitmapFilterViewVisitor(ILayerManager layerManager)
    {
        _layerManager = layerManager;
        Contents = new List<LayerViewModelBase>();
    }

    public List<LayerViewModelBase> Contents { get; }

    public void Visit(GaussianFilter layer)
    {
        var vm = new GaussianLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(MedianFilter layer)
    {
        var vm = new MedianLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(OtsuFilter layer)
    {
        var vm = new OtsuLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(SobelFilter layer)
    {
        var vm = new SobelLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(ThresholdFilter layer)
    {
        var vm = new ThresholdLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(BoxBlurFilter layer)
    {
        var vm = new BoxBlurLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(ChannelFilterLayer layer) { }

    public void Visit(GammaFilterLayer layer) { }

    public void Visit(SchemeConverterLayer layer) { }

    public void Visit(ContrastAdaptiveSharpening layer)
    {
        var vm = new ContrastAdaptiveSharpeningLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(ShiftLayer layer)
    {
        var vm = new ShiftLayerViewModel(layer, _layerManager);
        Contents.Add(vm);
    }

    public void Visit(CropLayer layer)
    {
        var vm = new CropLayerViewModel(_layerManager, layer);
        Contents.Add(vm);
    }

    public void Visit(NearestNeighbourScalingLayer layer)
    {
        var vm = new ScaleLayerViewModel(_layerManager, layer, "Nearest Neighbour");
        Contents.Add(vm);
    }

    public void Visit(BilinearScalingLayer layer)
    {
        var vm = new ScaleLayerViewModel(_layerManager, layer, "Bilinear");
        Contents.Add(vm);
    }

    public void Visit(Lanczos3ScalingLayer layer)
    {
        var vm = new ScaleLayerViewModel(_layerManager, layer, "Lanczos 3");
        Contents.Add(vm);
    }

    public void Visit(AutoCorrectionLayer layer) { }

    public void Visit(SplineScalingLayer layer)
    {
        var vm = new SplineScalingViewModel(_layerManager, layer);
        Contents.Add(vm);
    }

    public void Visit(AtkinsonDithering layer)
    {
        var vm = new ParameterlessLayerViewModel("Dithering (Atkinson)", _layerManager, layer);
        Contents.Add(vm);
    }

    public void Visit(FloydSteinbergDithering layer)
    {
        var vm = new ParameterlessLayerViewModel("Dithering (Floyd-Steinberg)", _layerManager, layer);
        Contents.Add(vm);
    }

    public void Visit(OrderedDithering layer)
    {
        var vm = new ParameterlessLayerViewModel("Dithering (Ordered)", _layerManager, layer);
        Contents.Add(vm);
    }

    public void Visit(RandomDithering layer)
    {
        var vm = new ParameterlessLayerViewModel("Dithering (Random)", _layerManager, layer);
        Contents.Add(vm);
    }
}