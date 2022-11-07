namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImageUpdater
{
    Task UpdateAsync(IBitmapImage image);
}