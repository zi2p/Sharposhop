using Sharposhop.Core.Model;

namespace Sharposhop.Core.Loading;

public interface IPictureLoader
{
    ValueTask<PictureData> LoadImageAsync(Stream data);
}