
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.FilterProvider;

public interface ILayerProvider
{
    string DisplayName { get; }

    ILayer Create();
}