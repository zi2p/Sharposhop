using Sharposhop.Core.Layers.Dithering;
using Sharposhop.Core.Layers.Filtering.Filters;
using Sharposhop.Core.Layers.Scaling;

namespace Sharposhop.Core.Layers;

public interface ILayerVisitor
{
    void Visit(GaussianFilter layer);
    void Visit(MedianFilter layer);
    void Visit(OtsuFilter layer);
    void Visit(SobelFilter layer);
    void Visit(ThresholdFilter layer);
    void Visit(BoxBlurFilter layer);
    void Visit(ChannelFilterLayer layer);
    void Visit(GammaFilterLayer layer);
    void Visit(SchemeConverterLayer layer);
    void Visit(ContrastAdaptiveSharpening layer);
    void Visit(ShiftLayer layer);
    void Visit(CropLayer layer);
    void Visit(NearestNeighbourScalingLayer layer);
    void Visit(BilinearScalingLayer layer);
    void Visit(Lanczos3ScalingLayer layer);
    void Visit(AutoCorrectionLayer layer);
    void Visit(SplineScalingLayer layer);
    void Visit(AtkinsonDithering layer);
    void Visit(FloydSteinbergDithering layer);
    void Visit(OrderedDithering layer);
    void Visit(RandomDithering layer);
}