using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Writing;

namespace Sharposhop.AvaloniaUI.Models;

public class GammaSettings
{
    public GammaModel GammaValue { get; set; } = 0;
    public GammaFilter Filter { get; } = new ();

    public IBitmapImageWriter GetWriter(GammaModel currentGamma)
    {
        return new GammaConvertWriter(GammaValue, currentGamma);
    }

    public IBitmapImageWriter GetWriterBalanced(GammaModel newGamma, GammaModel currentGamma)
    {
        return new GammaConvertWriter(newGamma, currentGamma);
    }
}