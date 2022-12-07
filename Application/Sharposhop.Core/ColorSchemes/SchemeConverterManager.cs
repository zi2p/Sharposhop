using Sharposhop.Core.PictureManagement;

namespace Sharposhop.Core.ColorSchemes;

public class SchemeConverterManager : ISchemeConverterProvider, ISchemeConverterUpdater
{
    private readonly IPictureParametersUpdateObserver _updateObserver;

    public SchemeConverterManager(ISchemeConverter converter, IPictureParametersUpdateObserver updateObserver)
    {
        _updateObserver = updateObserver;
        Converter = converter;
    }

    public ISchemeConverter Converter { get; private set; }

    public ValueTask UpdateAsync(ISchemeConverter converter, bool notify = true)
    {
        Converter = converter;
        return notify ? _updateObserver.OnPictureParametersUpdated() : ValueTask.CompletedTask;
    }
}