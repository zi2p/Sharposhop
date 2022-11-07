namespace Sharposhop.Core.SchemeConverters;

public interface ISchemeConverterProvider
{
    ISchemeConverter Converter { get; }
}