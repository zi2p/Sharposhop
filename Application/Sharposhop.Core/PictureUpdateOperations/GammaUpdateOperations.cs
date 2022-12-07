using Sharposhop.Core.Extensions;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.PictureUpdateOperations;

public class GammaUpdateOperations : IBasePictureUpdateOperation
{
    private readonly Gamma _newValue;

    public GammaUpdateOperations(Gamma newValue)
    {
        _newValue = newValue;
    }

    public ValueTask UpdatePictureAsync(IUpdatePicture picture)
    {
        if (picture.Gamma.Equals(_newValue))
            return ValueTask.CompletedTask;
        
        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            span[i] = span[i].WithGamma(picture.Gamma).WithoutGamma(_newValue);
        }

        picture.UpdateGamma(_newValue);
     
        return ValueTask.CompletedTask;
    }
}