namespace Sharposhop.Core.BitmapImages;

public interface IBitmapImageSaver
{
    void SaveTo(Stream stream);
}