using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages;

public interface IWritableBitmapImage : IBitmapImage
{
    ValueTask WriteFromAsync<T>(IEnumerable<PlaneCoordinate> coordinates, T writer, bool notify = true) where T : IBitmapImageWriter;
}