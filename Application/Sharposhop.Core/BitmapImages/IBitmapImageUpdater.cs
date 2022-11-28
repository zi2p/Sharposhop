namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImageUpdater
{
    Task UpdateAsync(IWritableBitmapImage image);
}