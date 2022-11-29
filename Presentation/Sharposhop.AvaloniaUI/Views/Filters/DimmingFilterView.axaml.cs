using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.AvaloniaUI.ViewModels.Filters;

namespace Sharposhop.AvaloniaUI.Views.Filters;

public partial class DimmingFilterView : ReactiveUserControl<DimmingFilterViewModel>
{
    public DimmingFilterView()
    {
        InitializeComponent();
    }
}