namespace Sharposhop.Core.Tools;

public interface IExceptionSink
{
    void Log(Exception exception);
}