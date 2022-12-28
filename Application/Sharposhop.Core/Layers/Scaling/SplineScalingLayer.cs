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

        foreach (var coordinate in _enumerationStrategy.Enumerate(Size))
        {
            var (x, y) = coordinate;

            var mappedX = (x + 0.5f) * picture.Size.Width / Size.Width - 0.5f;
            var mappedY = (y + 0.5f) * picture.Size.Height / Size.Height - 0.5f;

            var intLeft = (int)Math.Floor(mappedX - 2);
            var intRight = (int)Math.Ceiling(mappedX + 2);
            var intTop = (int)Math.Floor(mappedY - 2);
            var intBottom = (int)Math.Ceiling(mappedY + 2);

            using DisposableArray<float> xWeights = DisposableArray<float>.OfSize(intRight - intLeft + 1);
            using DisposableArray<float> yWeights = DisposableArray<float>.OfSize(intBottom - intTop + 1);

            Span<float> xWeightSpan = xWeights.AsSpan();
            Span<float> yWeightSpan = yWeights.AsSpan();

            for (var i = intLeft; i <= intRight; i++)
            {
                xWeightSpan[i - intLeft] = MNF(mappedX - i);
            }

            for (var i = intTop; i <= intBottom; i++)
            {
                yWeightSpan[i - intTop] = MNF(mappedY - i);
            }

            float weight = 0;

            float accFirst = 0;
            float accSecond = 0;
            float accThird = 0;

            for (var yy = intTop; yy <= intBottom; yy++)
            {
                for (var xx = intLeft; xx <= intRight; xx++)
                {
                    var curW = xWeightSpan[xx - intLeft] * yWeightSpan[yy - intTop];

                    var coord = PlaneCoordinate.Padded(xx, yy, picture.Size);
                    var index = _enumerationStrategy.AsContinuousIndex(coord, picture.Size);
                    var triplet = picture.AsSpan()[index];

                    accFirst += triplet.First * curW;
                    accSecond += triplet.Second * curW;
                    accThird += triplet.Third * curW;

                    weight += curW;
                }
            }

            var indexAll = _enumerationStrategy.AsContinuousIndex(coordinate, Size);
            _buffer.AsSpan()[indexAll] = new ColorTriplet(accFirst / weight, accSecond / weight, accThird / weight);
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