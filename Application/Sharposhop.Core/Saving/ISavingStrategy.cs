using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Saving;

public interface ISavingStrategy
{
    ValueTask SaveAsync(Stream stream, IPicture picture);
}