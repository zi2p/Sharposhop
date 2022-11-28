using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public interface IWritableBitmapImage : IBitmapImage
{
    new ColorTriplet this[int x, int y] { get; set; }
}