using Sharposhop.Core.BitmapImages;

namespace Sharposhop.Core.Saving;

public interface ISavingStrategy
{
    ValueTask SaveAsync(Stream stream, IReadBitmapImage image);
}