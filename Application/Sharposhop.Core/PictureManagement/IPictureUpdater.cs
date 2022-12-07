using Sharposhop.Core.Model;

namespace Sharposhop.Core.PictureManagement;

public interface IPictureUpdater
{
    ValueTask UpdateAsync(PictureData data);
}