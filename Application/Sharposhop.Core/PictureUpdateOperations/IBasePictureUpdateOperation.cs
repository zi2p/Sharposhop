using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.PictureUpdateOperations;

public interface IBasePictureUpdateOperation
{
    ValueTask UpdatePictureAsync(IUpdatePicture picture);
}