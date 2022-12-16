using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class GaussianLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public GaussianLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Gaussian";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(() =>
        {
            var viewModel = new CreateGaussianViewModel(layerManager, _enumerationStrategy);

            var window = new GaussianWindow
            {
                ViewModel = viewModel,
            };

            window.Show();
        });
    }
}