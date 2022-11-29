namespace Sharposhop.Core.BitmapImages.SchemeConversion;

public interface ISchemeConverterProvider
{
    ISchemeConverter Converter { get; }
}