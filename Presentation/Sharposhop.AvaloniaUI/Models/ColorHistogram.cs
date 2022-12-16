using System;
using System.Collections.Generic;
using System.Linq;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Pictures;

namespace Sharposhop.AvaloniaUI.Models;

public class ColorHistogram
{
    private readonly Dictionary<int, int> _colors;

    public ColorHistogram(IPicture picture, ComponentType type, INormalizer normalizer)
    {
        var data = picture.AsSpan();
        _colors = new Dictionary<int, int>();
        foreach (var colorTriplet in data)
        {
            var color = type switch
            {
                ComponentType.Red => normalizer.DeNormalize(colorTriplet.First),
                ComponentType.Green => normalizer.DeNormalize(colorTriplet.Second),
                ComponentType.Blue => normalizer.DeNormalize(colorTriplet.Third),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Incorrect type of component")
            };

            this[color]++;
        }
    }

    public int this[int key]
    {
        get => _colors.ContainsKey(key) ? _colors[key] : 0;
        private set => _colors[key] = value;
    }

    public (double[] values, double max) GetCounts()
    {
        var data = new List<double>();
        for (var brightness = 0; brightness < 256; brightness++)
        {
            var count = this[brightness];
            data.AddRange(Enumerable.Repeat<double>(brightness, count));
        }

        return (data.ToArray(), _colors.Values.Max());
    }
}

public enum ComponentType
{
    Red,
    Green,
    Blue
}