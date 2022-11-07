using System;
using Avalonia.Threading;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.Enums;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.Tools;

public class MessageBoxExceptionSink : IExceptionSink
{
    public void Log(Exception exception)
    {
        Dispatcher.UIThread.Post(() =>
        {
            IMsBoxWindow<ButtonResult> message = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Error occured", exception.Message + exception.StackTrace);
        
            message.Show();
        });
    }
}