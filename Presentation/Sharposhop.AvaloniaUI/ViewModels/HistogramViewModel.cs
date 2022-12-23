using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class HistogramViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly ILayerManager _layerManager;
    private decimal _ignore;

#pragma warning disable CS8618
    public HistogramViewModel() { }
#pragma warning restore CS8618

    public HistogramViewModel(
        MainWindowViewModel mainWindowViewModel,
        ILayerManager layerManager)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _layerManager = layerManager;
    }

    public decimal Ignore
    {
        get => _ignore;
        set
        {
            _ignore = value;
            this.RaisePropertyChanged();
        }
    }

    public static CultureInfo CultureInfo => CultureInfo.InvariantCulture;
    public static AutoCorrectionLayer? Layer { get; private set; }

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

    public async ValueTask AddAutoCorrection()
    {
        if (Layer is not null)
            await RemoveAutoCorrection();

        var (min, max) = await GetMinMaxValues();
        Layer = new AutoCorrectionLayer(min, max);
        await _layerManager.Add(Layer, false);
    }

    public ValueTask RemoveAutoCorrection()
    {
        if (Layer is null) return ValueTask.CompletedTask;
        _layerManager.Remove(Layer);
        Layer = null;
        return ValueTask.CompletedTask;
    }

    private async Task<(int min, int max)> GetMinMaxValues()
    {
        var hists = await GenerateHistograms();
        var mins = new double[hists.Length];
        var maxs = new double[hists.Length];

        for (var i = 0; i < hists.Length; i++)
        {
            var (values, _) = hists[i].GetCounts();
            Array.Sort(values);
            var index = (int)Math.Floor(_ignore * values.Length);
            mins[i] = values[index + 1];
            maxs[i] = values[^(index + 1)];
        }

        return ((int)mins.Min(), (int)maxs.Max());
    }
}