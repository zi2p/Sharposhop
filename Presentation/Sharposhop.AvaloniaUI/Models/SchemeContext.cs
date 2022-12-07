using System;
using Microsoft.Extensions.DependencyInjection;
using Sharposhop.Core.ChannelFiltering;
using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Normalization;

namespace Sharposhop.AvaloniaUI.Models;

public class SchemeContext
{
    private readonly IServiceProvider _provider;

    public SchemeContext(
        IServiceProvider provider,
        ISchemeConverterUpdater schemeConverterUpdater,
        IChannelFilterUpdater channelFilterUpdater,
        INormalizer normalizer)
    {
        _provider = provider;
        SchemeConverterUpdater = schemeConverterUpdater;
        ChannelFilterUpdater = channelFilterUpdater;
        Normalizer = normalizer;
    }

    public ISchemeConverterUpdater SchemeConverterUpdater { get; }
    public IChannelFilterUpdater ChannelFilterUpdater { get; }
    public INormalizer Normalizer { get; }

    public ISchemeConverter CreateConverter<T>() where T : ISchemeConverter
        => ActivatorUtilities.CreateInstance<T>(_provider);
}