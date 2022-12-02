using System.IO;
using System.Threading.Tasks;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class CreateGradientViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

#pragma warning disable CS8618
    public CreateGradientViewModel() { }
#pragma warning restore CS8618

    public CreateGradientViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        Width = 500;
        Height = 500;
        Red = 255;
        Green = 255;
        Blue = 255;
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }

    public async ValueTask GenerateAsync()
    {
        var request = $"grad {Width} {Height} {Red} {Green} {Blue}";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(request);
        await writer.FlushAsync();
        stream.Position = 0;

        await _mainWindowViewModel.LoadImageAsync(stream);
    }
}