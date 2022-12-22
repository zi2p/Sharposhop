using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class AutoCorrectionLayer : ILayer
{
    private int _minCoefficient;
    private int _maxCoefficient;

    public AutoCorrectionLayer(int minCoefficient, int maxCoefficient)
    {
        _minCoefficient = minCoefficient;
        _maxCoefficient = maxCoefficient;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        throw new NotImplementedException();
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}