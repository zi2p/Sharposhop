using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Scaling;

public class SplineScalingLayer : IScaleLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    private DisposableArray<ColorTriplet> _buffer;

    public SplineScalingLayer(IEnumerationStrategy enumerationStrategy, PictureSize size, float b, float c)
    {
        _enumerationStrategy = enumerationStrategy;
        Size = size;
        B = b;
        C = c;

        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public PictureSize Size { get; }
    public float B { get; }
    public float C { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (_buffer.Length < Size.PixelCount)
        {
            _buffer.Dispose();
            _buffer = DisposableArray<ColorTriplet>.OfSize(Size.PixelCount);
        }

        var widthRatio = (float)(picture.Size.Width - 1) / Size.Width;
        var heightRatio = (float)(picture.Size.Height - 1) / Size.Height;

        Span<ColorTriplet> span = picture.AsSpan();
        Span<ColorTriplet> bufferSpan = _buffer.AsSpan();

        foreach (var coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var x = (int)(coordinate.X * widthRatio);
            var y = (int)(coordinate.Y * heightRatio);

            var xDiff = (coordinate.X * widthRatio) - x;
            var yDiff = (coordinate.Y * heightRatio) - y;

            double first = 0;
            double second = 0;
            double third = 0;

            for (var k = -2; k <= 2; k++)
            {
                for (var l = -2; l <= 2; l++)
                {
                    var localCoordinate = PlaneCoordinate.Padded(x + k, y + l, picture.Size);
                    var index = _enumerationStrategy.AsContinuousIndex(localCoordinate, picture.Size);

                    var localTriplet = span[index];

                    first += localTriplet.First * MNF(xDiff - k) * MNF(yDiff - l);
                    second += localTriplet.Second * MNF(xDiff - k) * MNF(yDiff - l);
                    third += localTriplet.Third * MNF(xDiff - k) * MNF(yDiff - l);
                }
            }

            var triplet = new ColorTriplet((float)first, (float)second, (float)third);
            var bufferIndex = _enumerationStrategy.AsContinuousIndex(coordinate, Size);

            bufferSpan[bufferIndex] = triplet;
        }

        picture = new Picture(Size, picture.Scheme, picture.Gamma, _buffer);

        return ValueTask.FromResult(picture);
    }

    public void Reset()
    {
        _buffer.Dispose();
        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public void Accept(ILayerVisitor visitor)
    {
        visitor.Visit(this);
    }

    private float MNF(float x)
    {
        return Math.Abs(x) switch
        {
            < 1 => (12 - 9 * B - 6 * C) * MathF.Pow(Math.Abs(x), 3) +
                   (-18 + 12 * B + 6 * C) * MathF.Pow(Math.Abs(x), 2) + (6 - 2 * B),

            < 2 => (-B - 6 * C) * MathF.Pow(Math.Abs(x), 3) + (6 * B + 30 * C) * MathF.Pow(Math.Abs(x), 2) +
                   (-12 * B - 48 * C) * Math.Abs(x) + (8 * B + 24 * C),

            _ => 0,
        };
    }
}