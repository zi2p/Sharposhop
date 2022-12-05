namespace Sharposhop.Core.BitmapImages.SchemeConversion;

public interface ISchemeConverterUpdater
{
    ValueTask UpdateAsync(ISchemeConverter converter, bool notify = true);
}