using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class FloydSteinbergDithering : IDitheringLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public FloydSteinbergDithering(int depth, IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
        Depth = depth;
    }
    
    public int Depth { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var (width, height) = picture.Size;
        foreach (var coordinate in _enumerationStrategy.Enumerate(picture.Size).AsEnumerable())
        {
            var (x, y) = coordinate;
            var index = y * width + x;
            var (oldR, oldG, oldB) = span[index];
            var newPixel = new ColorTriplet(
                NormalizeValue(oldR),
                NormalizeValue(oldG),
                NormalizeValue(oldB));
            span[index] = newPixel;
            var quantError = (oldR - newPixel.First, oldG - newPixel.Second, oldB - newPixel.Third);

            ApplyError(quantError, x, y, index, width, height, span);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    private void ApplyError((float r, float g , float b) error,
        int x, int y, int index, int width, int height, Span<ColorTriplet> span)
    {
        if (x + 1 < width)
            span[index + 1] = new ColorTriplet(
                span[index + 1].First + error.r * 7f / 16,
                span[index + 1].Second + error.g * 7f / 16,
                span[index + 1].Third + error.b * 7f / 16);

        if (y + 1 >= height) return;
        if (x > 0)
            span[index + width - 1] = new ColorTriplet(
                span[index + width - 1].First + error.r * 3f / 16,
                span[index + width - 1].Second + error.g * 3f / 16,
                span[index + width - 1].Third + error.b * 3f / 16);

        span[index + width] = new ColorTriplet(
            span[index + width].First + error.r * 5f / 16,
            span[index + width].Second + error.g * 5f / 16,
            span[index + width].Third + error.b * 5f / 16);

        if (x + 1 < width)
            span[index + width + 1] = new ColorTriplet(
                span[index + width + 1].First + error.r * 1f / 16,
                span[index + width + 1].Second + error.g * 1f / 16,
                span[index + width + 1].Third + error.b * 1f / 16);
    }

    private float NormalizeValue(float value)
    {
        var step = 1 / (Math.Pow(2, Depth) - 1);
        var level = (int)(value / step);

        return (float)(step * level);
    }
}