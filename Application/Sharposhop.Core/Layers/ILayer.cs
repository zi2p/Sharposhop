using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public interface ILayer
{
    ValueTask<IPicture> ModifyAsync(IPicture picture);

    void Reset();

    void Accept(ILayerVisitor visitor);
}