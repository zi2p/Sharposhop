using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Sharposhop.AvaloniaUI.Views;

public partial class ErrorWindow : Window
{
    public ErrorWindow() { }

    public ErrorWindow(string title, string text, string? stackTrace)
    {
        InitializeComponent();

        Title.Text = title;
        TextBlock.Text = text;
        Stacktrace.Text = stackTrace;
    }

    private void Dismiss(object? sender, RoutedEventArgs e)
        => Close();

    private void ToggleStacktrace(object? sender, RoutedEventArgs e)
    {
        Stacktrace.IsVisible = !Stacktrace.IsVisible;
    }
}