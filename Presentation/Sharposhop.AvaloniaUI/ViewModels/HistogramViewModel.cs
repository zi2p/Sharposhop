using System.Threading.Tasks;
using Sharposhop.AvaloniaUI.Models;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class HistogramViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

#pragma warning disable CS8618
    public HistogramViewModel() { }
#pragma warning restore CS8618

    public HistogramViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    public async Task<ColorHistogram[]> GenerateHistograms()
    {
        var picture = await _mainWindowViewModel.PictureProvider.ComposePicture();
        if (_mainWindowViewModel.IsColored)
        {
            return new[]
            {
                new ColorHistogram(picture, ComponentType.Red),
                new ColorHistogram(picture, ComponentType.Green),
                new ColorHistogram(picture, ComponentType.Blue)
            };
        }

        return new[] { new ColorHistogram(picture, ComponentType.Red) };
    }
}