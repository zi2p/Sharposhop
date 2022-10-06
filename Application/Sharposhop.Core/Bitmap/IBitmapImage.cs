namespace Sharposhop.Core.Bitmap;

public interface IBitmapImage
{
    /// <summary>
    ///     Image stream in png format.
    /// </summary>
    Stream Stream { get; }
    
    int Width { get; }

    event Action BitmapChanged;
}