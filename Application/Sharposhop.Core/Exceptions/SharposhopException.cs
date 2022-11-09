namespace Sharposhop.Core.Exceptions;

public abstract class SharposhopException : Exception
{
    private protected SharposhopException(string message) : base(message) { }
}