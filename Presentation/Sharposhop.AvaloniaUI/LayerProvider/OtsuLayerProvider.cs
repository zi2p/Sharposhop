using System.Windows.Input;
using ReactiveUI;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;
using Sharposhop.Core.Normalization;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class OtsuLayerProvider : ILayerProvider
{
    private readonly INormalizer _normalizer;

    public OtsuLayerProvider(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public string DisplayName => "Otsu";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.CreateFromTask(async () => await layerManager.Add(new OtsuFilter(_normalizer)));
    }
}