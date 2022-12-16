using System;
using System.Drawing;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Statistics;
using Sharposhop.AvaloniaUI.ViewModels;

namespace Sharposhop.AvaloniaUI.Views;

public partial class HistogramWindow : ReactiveWindow<HistogramViewModel>
{
    public HistogramWindow()
    {
        InitializeComponent();
    }

    public AvaPlot HistogramPlot => this.Find<AvaPlot>("Histogram");

    private async void OnLoad(object? sender, EventArgs e)
    {
        var hists = await ViewModel!.GenerateHistograms();
        var plot = HistogramPlot;
        var (values, max) = hists[0].GetCounts();
        
        var (hist, binEdges) = Common.Histogram(values, 0, max, 1);
        var leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();
        
        plot.Plot.AddBar(hist, leftEdges, Color.Red);
        plot.Plot.SetAxisLimitsX(0, 255);
        plot.Plot.SetAxisLimitsY(0, max + 10);
        plot.Plot.Style(Style.Black);
        plot.Refresh();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}