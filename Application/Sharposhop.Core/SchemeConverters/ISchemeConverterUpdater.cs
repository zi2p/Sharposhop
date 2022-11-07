namespace Sharposhop.Core.SchemeConverters;

public interface ISchemeConverterUpdater
{
    Task UpdateAsync(ISchemeConverter converter, bool notify = true);
}