using Sharposhop.Core.Model;

namespace Sharposhop.Core.Layers.Scaling;

public interface IScaleLayer : ILayer
{
    PictureSize Size { get; }
}