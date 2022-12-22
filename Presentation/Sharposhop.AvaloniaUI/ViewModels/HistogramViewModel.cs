using System.Globalization;
using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Models;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class HistogramViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private float _ignore;

#pragma warning disable CS8618
    public HistogramViewModel() { }
#pragma warning restore CS8618

    public HistogramViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    public float Ignore
    {
        get => _ignore;
        set
        {
            _ignore = value;
            this.RaisePropertyChanged();
        }
    }

    public static CultureInfo CultureInfo => CultureInfo.InvariantCulture;

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

    public ValueTask AddAutoCorrection()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAutoCorrection()
    {
        return ValueTask.CompletedTask;
    }
}