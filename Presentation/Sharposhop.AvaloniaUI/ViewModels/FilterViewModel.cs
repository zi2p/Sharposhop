using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.AvaloniaUI.ViewModels.Layers;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels;

public sealed class FilterViewModel : ViewModelBase, IDisposable
{
    private readonly ILayerManager _manager;
    private readonly IExceptionSink _sink;
    private readonly SemaphoreSlim _semaphore;

    private LayerViewModelBase? _selected;

    public FilterViewModel(ILayerManager manager, IExceptionSink sink)
    {
        _manager = manager;
        _sink = sink;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public LayerViewModelBase? Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(ButtonsEnabled));
        }
    }

    public bool ButtonsEnabled => Selected is not null;

    public IEnumerable<LayerViewModelBase> Items
    {
        get
        {
            var visitor = new BitmapFilterViewVisitor(_sink);
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

            _manager.Promote(Selected.Filter);
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

            _manager.Demote(Selected.Filter);
            this.RaisePropertyChanged(nameof(Items));
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
        => _semaphore.Dispose();
}