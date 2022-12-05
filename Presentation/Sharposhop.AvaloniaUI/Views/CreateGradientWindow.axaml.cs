using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels;

namespace Sharposhop.AvaloniaUI.Views;

public partial class CreateGradientWindow : ReactiveWindow<CreateGradientViewModel>
{
    public CreateGradientWindow()
    {
        InitializeComponent();
    }

    private async void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await (ViewModel?.GenerateAsync() ?? ValueTask.CompletedTask);
        Hide();
    }
}