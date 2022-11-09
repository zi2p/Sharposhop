namespace Sharposhop.Core.Exceptions;

public class BitmapImageProxyException : SharposhopException
{
    private BitmapImageProxyException(string message) : base(message) { }

    internal static BitmapImageProxyException NoImageLoaded()
        => new BitmapImageProxyException("No image loaded");
}