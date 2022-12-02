using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Gamma;

namespace Sharposhop.AvaloniaUI.Models;

public class GammaSettings
{
    public GammaSettings(UserAction userAction)
    {
        Filter = new GammaFilter(userAction);
    }
    
    public bool IsSrgb { get; set; }
    public GammaModel GammaValue { get; set; } = 0; 
    public GammaFilter Filter { get; }

    public GammaModel EffectiveGamma => IsSrgb ? GammaModel.DefaultGamma : GammaValue;

    public GammaConvertWriter GetWriter(GammaModel currentGamma)
        => new GammaConvertWriter(EffectiveGamma, currentGamma);

    public GammaConvertWriter GetWriterBalanced(GammaModel newGamma, GammaModel currentGamma)
        => new GammaConvertWriter(newGamma, currentGamma);
}