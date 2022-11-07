using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.SchemeConverters;
using SkiaSharp;

namespace Sharposhop.Core.Loading;

public class SkiaImageLoader : IImageLoader
{
    private readonly INormalizer _normalizer;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ISchemeConverterProvider _schemeConverterProvider;

    public SkiaImageLoader(INormalizer normalizer, ISchemeConverterProvider schemeConverterProvider)
    {
        _normalizer = normalizer;
        _schemeConverterProvider = schemeConverterProvider;
        _enumerationStrategy = new RowByRowEnumerationStrategy();
    }

    public async Task<IBitmapImage> LoadImageAsync(Stream data)
    {
        using var stream = new MemoryStream();
        await data.CopyToAsync(stream);

        data.Position = 0;
        stream.Position = 0;

        using var imgStream = new SKManagedStream(data);
        using var skData = SKData.Create(data);
        using var codec = SKCodec.Create(skData);

        var bitmap = SKBitmap.Decode(codec);

        var values = new ColorTriplet[bitmap.Height * bitmap.Width];

        foreach (var (x, y) in _enumerationStrategy.Enumerate(bitmap.Width, bitmap.Height))
        {
            var pixel = bitmap.GetPixel(x, y);
                
            var triplet =  new ColorTriplet
            (
                _normalizer.Normalize(pixel.Red),
                _normalizer.Normalize(pixel.Green),
                _normalizer.Normalize(pixel.Blue)
            );

            var continuousIndex = _enumerationStrategy.AsContinuousIndex(x, y, bitmap.Width, bitmap.Height);
            values[continuousIndex] = _schemeConverterProvider.Converter.Revert(triplet);
        }

        return new RowByRowArrayBitmapImage(bitmap.Width, bitmap.Height, values);
    }
}