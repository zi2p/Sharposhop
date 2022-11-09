using System;
using Microsoft.Extensions.DependencyInjection;
using Sharposhop.Core.ChannelFilters;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.SchemeConverters;

namespace Sharposhop.AvaloniaUI.Models;

public class SchemeContext
{
    private readonly IServiceProvider _provider;

    public SchemeContext(
        IServiceProvider provider,
        ISchemeConverterUpdater schemeConverterUpdater,
        IChannelFilterUpdater channelFilterUpdater,
        IDeNormalizer deNormalizer)
    {
        _provider = provider;
        SchemeConverterUpdater = schemeConverterUpdater;
        ChannelFilterUpdater = channelFilterUpdater;
        DeNormalizer = deNormalizer;
    }

    public ISchemeConverterUpdater SchemeConverterUpdater { get; }
    public IChannelFilterUpdater ChannelFilterUpdater { get; }
    public IDeNormalizer DeNormalizer { get; }

    public ISchemeConverter CreateConverter<T>() where T : ISchemeConverter
        => ActivatorUtilities.CreateInstance<T>(_provider);
}