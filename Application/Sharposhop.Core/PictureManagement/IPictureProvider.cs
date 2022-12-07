using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.PictureManagement;

public interface IPictureProvider
{
    ValueTask<IPicture> ComposePicture();
}