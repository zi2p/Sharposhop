using System.Collections.Generic;
using Sharposhop.AvaloniaUI.ViewModels.Filters;
using Sharposhop.Core.BitmapImages.Filtering;
using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.Tools;

public class BitmapFilterViewVisitor : IBitmapFilterVisitor
{
    private readonly IExceptionSink _sink;

    public BitmapFilterViewVisitor(IExceptionSink sink)
    {
        _sink = sink;
    }

    public List<FilterViewModelBase> Contents { get; } = new List<FilterViewModelBase>();

    public void Visit(DimmingBitmapFilter filter)
    {
        var viewModel = new DimmingFilterViewModel(filter, _sink);
        Contents.Add(viewModel);
    }
}