using System.Collections.Generic;
using System.Windows.Input;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class MenuItemViewModel
{
    public MenuItemViewModel(string header)
    {
        Header = header;
        Items = new List<MenuItemViewModel>();
    }

    public string Header { get; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public IList<MenuItemViewModel> Items { get; init; }
}