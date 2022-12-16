using System;
using System.Threading.Tasks;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.Tools;

public sealed class LayerManagerSafeExecutionProxy : ILayerManager, IDisposable
{
    private readonly ILayerManager _layerManager;
    private readonly MainWindowViewModel _viewModel;

    public LayerManagerSafeExecutionProxy(ILayerManager layerManager, MainWindowViewModel viewModel)
    {
        _layerManager = layerManager;
        _viewModel = viewModel;

        layerManager.LayersUpdated += OnLayersUpdated;
    }

    public event Func<ValueTask>? LayersUpdated;

    public async ValueTask Add(ILayer layer, bool canReorder = true)
        => await _viewModel.ExecuteSafeAsync(async () => await _layerManager.Add(layer, canReorder));

    public async ValueTask Remove(ILayer layer)
        => await _viewModel.ExecuteSafeAsync(async () => await _layerManager.Remove(layer));

    public async ValueTask Promote(ILayer layer)
        => await _viewModel.ExecuteSafeAsync(async () => await _layerManager.Promote(layer));

    public async ValueTask Demote(ILayer layer)
        => await _viewModel.ExecuteSafeAsync(async () => await _layerManager.Demote(layer));

    public void Accept(ILayerVisitor visitor)
        => _layerManager.Accept(visitor);

    private ValueTask OnLayersUpdated()
        => LayersUpdated?.Invoke() ?? ValueTask.CompletedTask;

    public void Dispose()
    {
        _layerManager.LayersUpdated -= OnLayersUpdated;
    }
}