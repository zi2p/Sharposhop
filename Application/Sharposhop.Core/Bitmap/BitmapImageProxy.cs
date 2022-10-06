namespace Sharposhop.Core.Bitmap;

public class BitmapImageProxy : IBitmapImage, IBitmapUpdater
{
    private IBitmapImage _image;

    public BitmapImageProxy(IBitmapImage image)
    {
        _image = image;

        image.BitmapChanged += BitmapChanged;
    }

    ~BitmapImageProxy()
    {
        _image.BitmapChanged -= BitmapChanged;
    }

    public Stream Stream => _image.Stream;
    public int Width => _image.Width;

    public event Action? BitmapChanged;

    public void Update(IBitmapImage image)
    {
        _image.BitmapChanged -= BitmapChanged;

        _image = image;
        _image.BitmapChanged += BitmapChanged;

        BitmapChanged?.Invoke();
    }
}