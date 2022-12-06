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

    public async ValueTask<IReadBitmapImage> LoadImageAsync(Stream data)
    {
        var type = await RecognizeImageTypeAsync(data);
        data.Position = 0;
        
        var loader = _loaderFactory.CreateRightImageLoader(type);
        return await loader.LoadImageAsync(data);
    }

    private async ValueTask<ImageFileTypes> RecognizeImageTypeAsync(Stream data)
    {
        if (await IsImageTypeGradient(data))
            return ImageFileTypes.Gradient;
        if (await IsImageTypePnm(data))
            return ImageFileTypes.Pnm;
        if (await IsImageTypeBmp(data))
            return ImageFileTypes.Bmp;

        return ImageFileTypes.Other;
    }

    /// <summary>
    ///     format: grad x y r g b
    /// </summary>
    private async ValueTask<bool> IsImageTypeGradient(Stream data)
    {
        data.Position = 0;

        if (data.Length < 4)
            return false;

        var grad = new byte[4];

        _ = await data.ReadAsync(grad.AsMemory(0, 4));
        var gradString = Encoding.UTF8.GetString(grad);

        return gradString is "grad";
    }

    private async ValueTask<bool> IsImageTypeBmp(Stream data)
    {
        data.Position = 0;

        if (data.Length <= BmpHeaderLength)
            return false;

        var bmpHeaderField = new byte[BmpHeaderFieldLength];
        _ = await data.ReadAsync(bmpHeaderField.AsMemory(0, BmpHeaderFieldLength));
        var bmpHeaderFieldString = Encoding.UTF8.GetString(bmpHeaderField);

        return bmpHeaderFieldString is "BM" or "BA" or "CI" or "CI" or "CP" or "PT";
    }

    private async ValueTask<bool> IsImageTypePnm(Stream data)
    {
        data.Position = 0;

        if (data.Length <= PnmHeaderLength)
            return false;

        var pnmHeaderField = new byte[PnmHeaderLength];
        _ = await data.ReadAsync(pnmHeaderField.AsMemory(0, PnmHeaderLength));
        var pnmHeaderFieldString = Encoding.UTF8.GetString(pnmHeaderField);

        return pnmHeaderFieldString is "P5" or "P6";
    }
}