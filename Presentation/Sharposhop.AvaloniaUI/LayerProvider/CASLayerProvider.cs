using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class CASLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public CASLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Contrast Adaptive Sharpening";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(() =>
        {
            var viewModel = new CreateContrastAdaptiveSharpeningViewModel(layerManager, _enumerationStrategy);

            var window = new ContrastAdaptiveSharpeningWindow
            {
                ViewModel = viewModel,
            };

            window.Show();
        });
    }
}