
using System.Windows.Input;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public interface ILayerProvider
{
    string DisplayName { get; }

    ICommand Create(ILayerManager layerManager);
}