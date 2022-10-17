using System.Text;
using Sharposhop.Core.Bitmap;
using Sharposhop.Core.Tools;

namespace Sharposhop.Core.Loading;

public class LoaderProxy : IImageLoader
{
    private const int BmpHeaderLength = 14;
    private const int BmpHeaderFieldLength = 2;
    private const int PnmHeaderLength = 2;

    private readonly LoaderFactory _loaderFactory;

    public LoaderProxy(LoaderFactory loaderFactory)
    {
        _loaderFactory = loaderFactory;
    }

    public async Task<IBitmapImage> LoadImageAsync(string path)
    {
        var type = await RecognizeImageTypeAsync(path);
        var loader = _loaderFactory.CreateRightImageLoader(type);
        return await loader.LoadImageAsync(path);
    }

    private async Task<ImageFileTypes> RecognizeImageTypeAsync(string path)
    {
        if (await IsImageTypePnm(path)) return ImageFileTypes.Pnm;
        if (await IsImageTypeBmp(path)) return ImageFileTypes.Bmp;
        return ImageFileTypes.Other;
    }

    private async Task<bool> IsImageTypeBmp(string path)
    {
        await using var fileStream = new FileStream(path, FileMode.Open);
        if (fileStream.Length <= BmpHeaderLength) return false;

        var bmpHeaderField = new byte[BmpHeaderFieldLength];
        await fileStream.ReadAsync(bmpHeaderField.AsMemory(0, BmpHeaderFieldLength));
        var bmpHeaderFieldString = Encoding.UTF8.GetString(bmpHeaderField);

        return bmpHeaderFieldString is "BM" or "BA" or "CI" or "CI" or "CP" or "PT";
    }

    private async Task<bool> IsImageTypePnm(string path)
    {
        await using var fileStream = new FileStream(path, FileMode.Open);
        if (fileStream.Length <= PnmHeaderLength) return false;

        var pnmHeaderField = new byte[PnmHeaderLength];
        await fileStream.ReadAsync(pnmHeaderField.AsMemory(0, PnmHeaderLength));
        var pnmHeaderFieldString = Encoding.UTF8.GetString(pnmHeaderField);

        return pnmHeaderFieldString is "P5" or "P6";
    }
}