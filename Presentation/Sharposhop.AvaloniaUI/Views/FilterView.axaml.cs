using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels;

namespace Sharposhop.AvaloniaUI.Views;

public partial class FilterView : ReactiveUserControl<FilterViewModel>
{
    public FilterView()
    {
        InitializeComponent();
    }

    private void PromoteClicked(object? sender, RoutedEventArgs e)
    {
        ViewModel?.Promote();
    }
    
    private void DemoteClicked(object? sender, RoutedEventArgs e)
    {
        ViewModel?.Demote();
    }
}