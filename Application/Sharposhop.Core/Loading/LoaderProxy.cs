using System.Text;
using Sharposhop.Core.BitmapImages;
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

    public async Task<IBitmapImage> LoadImageAsync(Stream data)
    {
        var type = await RecognizeImageTypeAsync(data);
        data.Position = 0;
        
        var loader = _loaderFactory.CreateRightImageLoader(type);
        return await loader.LoadImageAsync(data);
    }

    private async Task<ImageFileTypes> RecognizeImageTypeAsync(Stream data)
    {
        if (await IsImageTypePnm(data)) return ImageFileTypes.Pnm;
        if (await IsImageTypeBmp(data)) return ImageFileTypes.Bmp;
        return ImageFileTypes.Other;
    }

    private async Task<bool> IsImageTypeBmp(Stream data)
    {
        data.Position = 0;
        
        if (data.Length <= BmpHeaderLength) return false;

        var bmpHeaderField = new byte[BmpHeaderFieldLength];
        _ = await data.ReadAsync(bmpHeaderField.AsMemory(0, BmpHeaderFieldLength));
        var bmpHeaderFieldString = Encoding.UTF8.GetString(bmpHeaderField);

        return bmpHeaderFieldString is "BM" or "BA" or "CI" or "CI" or "CP" or "PT";
    }

    private async Task<bool> IsImageTypePnm(Stream data)
    {
        data.Position = 0;
        
        if (data.Length <= PnmHeaderLength) return false;

        var pnmHeaderField = new byte[PnmHeaderLength];
        _ = await data.ReadAsync(pnmHeaderField.AsMemory(0, PnmHeaderLength));
        var pnmHeaderFieldString = Encoding.UTF8.GetString(pnmHeaderField);

        return pnmHeaderFieldString is "P5" or "P6";
    }
}