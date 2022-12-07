namespace Sharposhop.Core.ColorSchemes;

public interface ISchemeConverterProvider
{
    ISchemeConverter Converter { get; }
}