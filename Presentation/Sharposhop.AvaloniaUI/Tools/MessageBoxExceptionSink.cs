using System;
using Avalonia.Threading;
using Sharposhop.AvaloniaUI.Views;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.Tools;

public class MessageBoxExceptionSink : IExceptionSink
{
    public void Log(Exception exception)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var message = new ErrorWindow("Error occured", exception.Message, exception.StackTrace);
            message.Show();
        });
    }
}