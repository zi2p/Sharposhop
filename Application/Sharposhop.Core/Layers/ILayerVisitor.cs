using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.Core.Layers;

public interface ILayerVisitor
{
    void Visit(GaussianFilter layer);
    void Visit(MedianFilter layer);
    void Visit(OtsuFilter layer);
    void Visit(SobelFilter layer);
    // void Visit(ContrastAdaptiveSharpening layer);
    void Visit(ThresholdFilter layer);
    void Visit(BoxBlurFilter layer);
    void Visit(ChannelFilterLayer layer);

    void Visit(GammaFilterLayer layer);

    void Visit(SchemeConverterLayer layer);
    void Visit(ContrastAdaptiveSharpening layer);
}