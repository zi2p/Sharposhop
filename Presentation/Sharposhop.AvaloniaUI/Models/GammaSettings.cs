using Sharposhop.Core.GammaConfiguration;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.Models;

public class GammaSettings
{
    public GammaSettings(IGammaUpdater gammaUpdater)
    {
        GammaUpdater = gammaUpdater;
    }

    public bool IsSrgb { get; set; }
    public Gamma GammaValue { get; set; } = 0;
    public IGammaUpdater GammaUpdater { get; }

    public Gamma EffectiveGamma => IsSrgb ? Gamma.DefaultGamma : GammaValue;
}