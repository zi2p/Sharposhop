using System.Collections.Generic;
using System.Windows.Input;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class MenuItemViewModel : ViewModelBase
{
    public MenuItemViewModel(string header)
    {
        Header = header;
        Items = new List<ViewModelBase>();
    }

    public string Header { get; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public IList<ViewModelBase> Items { get; init; }
}