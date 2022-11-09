namespace Sharposhop.Core.Exceptions;

public class WrongFileFormatException : SharposhopException
{
    private WrongFileFormatException(string message) : base(message) { }

    internal static WrongFileFormatException ImageTypeNotSupported()
    {
        return new WrongFileFormatException("This type of image is not supported in current version");
    }

    internal static WrongFileFormatException IncorrectFileContent()
    {
        return new WrongFileFormatException("File content has incorrect format");
    }
}