using System;
using ReactiveUI;
using Sharposhop.Core.BitmapImages.Filtering;
using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels.Filters;

public class DimmingFilterViewModel : FilterViewModelBase
{
    private readonly DimmingBitmapFilter _filter;
    private readonly IExceptionSink _sink;

    public DimmingFilterViewModel(DimmingBitmapFilter filter, IExceptionSink sink)
    {
        _filter = filter;
        _sink = sink;
    }

    public string Name => _filter.DisplayName;
    public override IBitmapFilter Filter => _filter;

    public string Value
    {
        get => _filter.Value.ToString();
        set
        {
            try
            {
                _filter.Value = float.Parse(value);
                this.RaisePropertyChanged();
            }
            catch (Exception e)
            {
                _sink.Log(e);
                this.RaisePropertyChanged();
            }
        }
    }
}