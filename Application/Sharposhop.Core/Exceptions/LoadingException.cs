namespace Sharposhop.Core.Exceptions;

public class LoadingException : SharposhopException
{
    private LoadingException(string message) : base(message) { }

    internal static LoadingException UnexpectedStreamEnd()
        => new LoadingException("Stream ended unexpectedly");
}