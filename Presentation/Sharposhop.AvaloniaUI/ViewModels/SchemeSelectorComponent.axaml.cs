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
        SchemeContext context,
        Func<SchemeContext, ISchemeConverter> converterFactory)
    {
        _context = context;
        _converterFactory = converterFactory;
        Title = $"_{title}";
    }

    public string Title { get; }

    public async Task SchemeSelectedAsync()
    {
        var scheme = _converterFactory.Invoke(_context);
        var filter = new PassthroughChannelFilter(_context.DeNormalizer);

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
        var filter = new SingleChannelFilter(channel, _context.DeNormalizer);

        await _context.SchemeConverterUpdater.UpdateAsync(scheme, false);
        await _context.ChannelFilterUpdater.UpdateAsync(filter);
    }
}