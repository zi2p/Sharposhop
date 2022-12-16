using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class ThresholdLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public ThresholdLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Threshold";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(() =>
        {
            var viewModel = new CreateThresholdViewModel(layerManager, _enumerationStrategy);

            var window = new ThresholdWindow
            {
                ViewModel = viewModel,
            };

            window.Show();
        });
    }
}