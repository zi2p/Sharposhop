using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Gamma;

namespace Sharposhop.AvaloniaUI.Models;

public class GammaSettings
{
    public GammaSettings(UserAction userAction)
    {
        BitmapFilter = new GammaBitmapFilter(userAction);
    }
    
    public bool IsSrgb { get; set; }
    public GammaModel GammaValue { get; set; } = 0; 
    public GammaBitmapFilter BitmapFilter { get; }

    public GammaModel EffectiveGamma => IsSrgb ? GammaModel.DefaultGamma : GammaValue;
}