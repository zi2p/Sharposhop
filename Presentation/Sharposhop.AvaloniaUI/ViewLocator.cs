using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Sharposhop.AvaloniaUI.ViewModels;

namespace Sharposhop.AvaloniaUI;

public class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public IControl Build(object data)
    {
        var name = data.GetType().FullName?.Replace("ViewModel", "View");
        Type? type = name is null ? null : Type.GetType(name);

        if (type is not null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object data)
        => data is ViewModelBase;
}