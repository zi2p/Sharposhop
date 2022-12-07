using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.PictureManagement;

public interface IPictureUpdateObserver
{
    ValueTask OnPictureUpdated(IPicture picture);
}