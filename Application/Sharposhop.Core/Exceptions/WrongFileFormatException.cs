namespace Sharposhop.Core.Exceptions;

public class WrongFileFormatException : Exception
{
    private WrongFileFormatException(string message) : base(message) { }

    public static WrongFileFormatException ImageTypeNotSupported()
    {
        return new WrongFileFormatException("This type of image is not supported in current version");
    }

    public static WrongFileFormatException IncorrectFileContent()
    {
        return new WrongFileFormatException("File content has incorrect format");
    }
}