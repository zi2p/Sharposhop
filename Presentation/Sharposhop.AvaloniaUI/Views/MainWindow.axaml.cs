using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels;

namespace Sharposhop.AvaloniaUI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoadFileButton_OnClick(object? sender, RoutedEventArgs e)
        => ViewModel?.LoadImageAsync(this);

    private void SaveFileBmpButton_Clicked(object? sender, RoutedEventArgs e)
        => ViewModel?.SaveImageBmpAsync(this);

    private void SaveFileP5Button_Clicked(object? sender, RoutedEventArgs e)
        => ViewModel?.SaveImageP5Async(this);
    
    private void SaveFileP6Button_Clicked(object? sender, RoutedEventArgs e)
        => ViewModel?.SaveImageP6Async(this);
}