using Sharposhop.Core.Bitmap;

namespace Sharposhop.Core.Saving;

public class SaverProxy : IImageSaver
{
    public async Task SaveAsync(IBitmapImage image, string path, SaveMode mode)
    {
        if (mode is SaveMode.P5 or SaveMode.P6) await new PnmSaver().SaveAsync(image, path, mode);
        if (mode is SaveMode.Bmp) await new BmpSaver().SaveAsync(image, path, mode);
    }
}