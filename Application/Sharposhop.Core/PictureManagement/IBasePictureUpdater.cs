using Sharposhop.Core.PictureUpdateOperations;

namespace Sharposhop.Core.PictureManagement;

public interface IBasePictureUpdater
{
    ValueTask UpdateAsync<T>(T operation, bool notify = true) where T : IBasePictureUpdateOperation;
}