namespace Sharposhop.Core.Model;

public record struct PictureData(PictureSize Size, ColorScheme Scheme, Gamma Gamma, ColorTriplet[] Data);