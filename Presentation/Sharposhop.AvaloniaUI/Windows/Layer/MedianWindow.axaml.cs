using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;

namespace Sharposhop.AvaloniaUI.Windows.Layer;

public partial class MedianWindow : ReactiveWindow<CreateMedianViewModel>
{
    public MedianWindow()
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
        Close();
        await (ViewModel?.Add() ?? ValueTask.CompletedTask);
    }
}