using System.Collections.Generic;
using Sharposhop.AvaloniaUI.ViewModels.Layers;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.Tools;

public class BitmapFilterViewVisitor : ILayerVisitor
{
    private readonly IExceptionSink _sink;

    public BitmapFilterViewVisitor(IExceptionSink sink)
    {
        _sink = sink;
        Contents = new List<LayerViewModelBase>();
    }

    public List<LayerViewModelBase> Contents { get; }

    public void Visit(GaussianFilter layer) { }

    public void Visit(MedianFilter layer) { }

    public void Visit(OtsuFilter layer) { }

    public void Visit(SobelFilter layer) { }

    public void Visit(ThresholdFilter layer) { }

    public void Visit(BoxBlurFilter layer) { }

    public void Visit(ChannelFilterLayer layer) { }

    public void Visit(GammaFilterLayer layer) { }

    public void Visit(SchemeConverterLayer layer) { }
    public void Visit(ContrastAdaptiveSharpening layer) { }
}