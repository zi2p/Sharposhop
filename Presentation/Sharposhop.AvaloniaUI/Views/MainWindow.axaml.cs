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

    private void SaveFileButton_Clicked(object? sender, RoutedEventArgs e)
        => ViewModel?.SaveImageAsync(this);
}