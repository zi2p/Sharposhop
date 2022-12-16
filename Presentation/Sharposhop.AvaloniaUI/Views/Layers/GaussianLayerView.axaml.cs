using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Layers;

namespace Sharposhop.AvaloniaUI.Views.Layers;

public partial class GaussianLayerView : ReactiveUserControl<GaussianLayerViewModel>
{
    public GaussianLayerView()
    {
        InitializeComponent();
    }

    private async void Remove(object? sender, RoutedEventArgs e)
        => await (ViewModel?.RemoveAsync() ?? ValueTask.CompletedTask);
}