using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.AvaloniaUI.ViewModels.Layers;
using Sharposhop.Core.LayerManagement;

namespace Sharposhop.AvaloniaUI.ViewModels;

public sealed class FilterViewModel : ViewModelBase, IDisposable
{
    private readonly ILayerManager _manager;
    private readonly SemaphoreSlim _semaphore;
    private readonly ISelectedLayerManager _selectedLayerManager;

    private LayerViewModelBase? _selected;

    public FilterViewModel(ILayerManager manager, ISelectedLayerManager selectedLayerManager)
    {
        _manager = manager;
        _selectedLayerManager = selectedLayerManager;
        _semaphore = new SemaphoreSlim(1, 1);

        manager.LayersUpdated += OnManagerOnLayersUpdated;
    }

    private ValueTask OnManagerOnLayersUpdated()
    {
        this.RaisePropertyChanged(nameof(Items));
        return ValueTask.CompletedTask;
    }

    public LayerViewModelBase? Selected
    {
        get => _selected;
        set
        {
            _selected = _selected?.Equals(value) is true ? null : value;

            if (_selected is not null)
                _selectedLayerManager.UpdateSelectedLayer(_selected.Layer);

            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(ButtonsEnabled));
        }
    }

    public bool ButtonsEnabled => Selected is not null;

    public IEnumerable<LayerViewModelBase> Items
    {
        get
        {
            var visitor = new BitmapFilterViewVisitor(_manager);
            _manager.Accept(visitor);

            return visitor.Contents;
        }
    }

    public async ValueTask Promote()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (Selected is null)
                return;

            await _manager.Promote(Selected.Layer);
            this.RaisePropertyChanged(nameof(Items));
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask Demote()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (Selected is null)
                return;

            await _manager.Demote(Selected.Layer);
            this.RaisePropertyChanged(nameof(Items));
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _manager.LayersUpdated -= OnManagerOnLayersUpdated;
    }
}