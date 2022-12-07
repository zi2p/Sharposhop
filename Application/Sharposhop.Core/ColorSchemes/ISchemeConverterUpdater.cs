namespace Sharposhop.Core.ColorSchemes;

public interface ISchemeConverterUpdater
{
    ValueTask UpdateAsync(ISchemeConverter converter, bool notify = true);
}