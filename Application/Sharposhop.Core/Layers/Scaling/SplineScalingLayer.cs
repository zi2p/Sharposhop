using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Scaling;

public class SplineScalingLayer : IScaleLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public SplineScalingLayer(IEnumerationStrategy enumerationStrategy, PictureSize size, float b, float c)
    {
        _enumerationStrategy = enumerationStrategy;
        Size = size;
        B = b;
        C = c;
    }

    public PictureSize Size { get; }
    public float B { get; }
    public float C { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
    {
        visitor.Visit(this);
    }
}