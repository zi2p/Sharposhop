using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sharposhop.AvaloniaUI.Windows.Layer;

public partial class ContrastAdaptiveSharpningWindow : Window
{
    public ContrastAdaptiveSharpningWindow()
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
}