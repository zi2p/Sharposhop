using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Layers;

namespace Sharposhop.AvaloniaUI.Views.Layers;

public partial class DitheringView : ReactiveUserControl<DitheringViewModel>
{
    public DitheringView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void Remove(object? sender, RoutedEventArgs e)
        => await (ViewModel?.RemoveAsync() ?? ValueTask.CompletedTask);
}