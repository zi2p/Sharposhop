using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class MedianLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public MedianLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Median";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(() =>
        {
            var viewModel = new CreateMedianViewModel(layerManager, _enumerationStrategy);

            var window = new MedianWindow
            {
                ViewModel = viewModel,
            };

            window.Show();
        });
    }
}