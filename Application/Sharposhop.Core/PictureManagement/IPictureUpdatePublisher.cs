namespace Sharposhop.Core.PictureManagement;

public interface IPictureUpdatePublisher
{
    void Subscribe(IPictureUpdateObserver observer);
    void Unsubscribe(IPictureUpdateObserver observer);
}