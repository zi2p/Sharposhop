using System;
using System.Threading.Tasks;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.ChannelFiltering.Filters;
using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class SchemeSelectorComponent
{
    private readonly Func<SchemeContext, ISchemeConverter> _converterFactory;
    private readonly SchemeContext _context;

    public SchemeSelectorComponent(
        string title,
        string firstComponentName,
        string secondComponentName,
        string thirdComponentName,
        SchemeContext context,
        Func<SchemeContext, ISchemeConverter> converterFactory)
    {
        _context = context;
        _converterFactory = converterFactory;
        FirstComponentName = firstComponentName;
        SecondComponentName = secondComponentName;
        ThirdComponentName = thirdComponentName;
        Title = $"_{title}";
    }

    public string Title { get; }

    public string FirstComponentName { get; }
    public string SecondComponentName { get; }
    public string ThirdComponentName { get; }

    public async Task SchemeSelectedAsync()
    {
        ISchemeConverter? scheme = _converterFactory.Invoke(_context);
        var filter = new PassthroughChannelFilter();

        await _context.SchemeConverterUpdater.UpdateAsync(scheme, false);
        await _context.ChannelFilterUpdater.UpdateAsync(filter);
    }

    public Task FirstChannelSelectedAsync()
        => ChannelSelectedAsync(Channel.First);

    public Task SecondChannelSelectedAsync()
        => ChannelSelectedAsync(Channel.Second);

    public Task ThirdChannelSelectedAsync()
        => ChannelSelectedAsync(Channel.Third);

    private async Task ChannelSelectedAsync(Channel channel)
    {
        ISchemeConverter? scheme = _converterFactory.Invoke(_context);
        var filter = new SingleChannelFilter(channel);

        await _context.SchemeConverterUpdater.UpdateAsync(scheme, false);
        await _context.ChannelFilterUpdater.UpdateAsync(filter);
    }
}