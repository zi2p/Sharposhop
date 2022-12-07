namespace Sharposhop.Core.Layers;

public interface ILayerVisitor
{
    void Visit(ChannelFilterLayer layer);

    void Visit(GammaFilterLayer layer);

    void Visit(SchemeConverterLayer layer);
}