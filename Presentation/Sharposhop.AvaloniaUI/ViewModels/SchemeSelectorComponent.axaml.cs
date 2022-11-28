using System;
using System.Threading.Tasks;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.ChannelFilters;
using Sharposhop.Core.Model;
using Sharposhop.Core.SchemeConverters;

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
        var scheme = _converterFactory.Invoke(_context);
        var filter = new PassthroughChannelFilter(_context.Normalizer);

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
        var scheme = _converterFactory.Invoke(_context);
        var filter = new SingleChannelFilter(channel, _context.Normalizer);

        await _context.SchemeConverterUpdater.UpdateAsync(scheme, false);
        await _context.ChannelFilterUpdater.UpdateAsync(filter);
    }
}