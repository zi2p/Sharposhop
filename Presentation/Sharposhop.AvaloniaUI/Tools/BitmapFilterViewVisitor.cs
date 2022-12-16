using System.Collections.Generic;
using Sharposhop.AvaloniaUI.ViewModels.Layers;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

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
}