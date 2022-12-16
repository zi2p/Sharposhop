using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class BoxBlurLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public BoxBlurLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Box Blur";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(() =>
        {
            var viewModel = new CreateBoxBlurViewModel(layerManager, _enumerationStrategy);

            var window = new BoxBlurWindow
            {
                ViewModel = viewModel,
            };

            window.Show();
        });
    }
}