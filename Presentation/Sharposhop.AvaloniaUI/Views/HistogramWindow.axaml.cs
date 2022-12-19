using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Statistics;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.AvaloniaUI.ViewModels;

namespace Sharposhop.AvaloniaUI.Views;

public partial class HistogramWindow : ReactiveWindow<HistogramViewModel>
{
    public HistogramWindow()
    {
        InitializeComponent();
    }

    public AvaPlot HistogramPlotRed => this.Find<AvaPlot>("HistogramRed");
    public AvaPlot HistogramPlotGreen => this.Find<AvaPlot>("HistogramGreen");
    public AvaPlot HistogramPlotBlue => this.Find<AvaPlot>("HistogramBlue");
    public static CultureInfo Culture => CultureInfo.InvariantCulture;
    public float Ignore { get; set; }


    private async void OnLoad(object? sender, EventArgs e)
    {
        var hists = await ViewModel!.GenerateHistograms();
        if (hists.Length == 3)
        {
            this.Find<Grid>("Grid").RowDefinitions = new RowDefinitions("*, *, *, 50");
            PlotHistogram(hists[0], HistogramPlotRed, Color.Red);
            PlotHistogram(hists[1], HistogramPlotGreen, Color.Green);
            PlotHistogram(hists[2], HistogramPlotBlue, Color.Blue);
            return;
        }

        HistogramPlotGreen.IsVisible = false;
        HistogramPlotBlue.IsVisible = false;
        this.Find<Grid>("Grid").RowDefinitions = new RowDefinitions("*, 0, 0, 50");

        PlotHistogram(hists[0], HistogramPlotRed, Color.GhostWhite);
    }

    private void PlotHistogram(ColorHistogram histogram, AvaPlot plot, Color color)
    {
        plot.IsVisible = true;
        var (values, max) = histogram.GetCounts();
        
        var (hist, binEdges) = Common.Histogram(values, 0, max, 1);
        var leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();
        
        plot.Plot.AddBar(hist, leftEdges, color);
        plot.Plot.SetAxisLimitsX(0, 255);
        plot.Plot.SetAxisLimitsY(0, max + 10);
        plot.Plot.Style(Style.Black);
        plot.Refresh();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}