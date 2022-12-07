using Sharposhop.Core.AppStateManagement;
using Sharposhop.Core.Extensions;
using Sharposhop.Core.GammaConfiguration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class GammaFilterLayer : ILayer
{
    private readonly IGammaProvider _provider;
    private readonly IAppStateProvider _stateProvider;

    public GammaFilterLayer(IGammaProvider provider, IAppStateProvider stateProvider)
    {
        _provider = provider;
        _stateProvider = stateProvider;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (_stateProvider.IsCurrentlySaving)
            return ValueTask.FromResult(picture);

        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            span[i] = span[i].WithoutGamma(Gamma.DefaultGamma).WithGamma(_provider.Gamma);
        }

        picture = new GammaPictureProxy(picture, _provider.Gamma);
        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}