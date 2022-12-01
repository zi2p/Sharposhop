using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Writing;

namespace Sharposhop.AvaloniaUI.Models;

public class GammaSettings
{
    private readonly UserAction _userAction;
    private readonly GammaFilter _filter;

    public GammaSettings(UserAction userAction)
    {
        _userAction = userAction;
        _filter = new (_userAction);
    }
    public GammaModel GammaValue { get; set; } = 0;
    public GammaFilter Filter => _filter;

    public IBitmapImageWriter GetWriter(GammaModel currentGamma)
    {
        return new GammaConvertWriter(GammaValue, currentGamma);
    }

    public IBitmapImageWriter GetWriterBalanced(GammaModel newGamma, GammaModel currentGamma)
    {
        return new GammaConvertWriter(newGamma, currentGamma);
    }
}