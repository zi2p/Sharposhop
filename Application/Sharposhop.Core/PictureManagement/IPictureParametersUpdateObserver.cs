namespace Sharposhop.Core.PictureManagement;

public interface IPictureParametersUpdateObserver
{
    ValueTask OnPictureParametersUpdated();
}