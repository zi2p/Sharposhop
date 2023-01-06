using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

namespace Sharposhop.AvaloniaUI.Windows.Layer;

public partial class DitheringLayerWindow : ReactiveWindow<CreateDitheringLayerViewModel>
{
    public DitheringLayerWindow()
    {
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        await (ViewModel?.AddAsync() ?? ValueTask.CompletedTask);
        Close();
    }
}