namespace Sharposhop.Core.Model;

public readonly record struct PictureSize(int Width, int Height)
{
    public int PixelCount => Width * Height;
}