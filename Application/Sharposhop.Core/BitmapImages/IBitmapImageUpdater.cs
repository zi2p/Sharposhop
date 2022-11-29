namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImageUpdater
{
    ValueTask UpdateAsync(IWritableBitmapImage image);
}