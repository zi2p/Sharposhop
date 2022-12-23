using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Scaling;

public class Lanczos3ScalingLayer : IScaleLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    private DisposableArray<ColorTriplet> _buffer;

    public Lanczos3ScalingLayer(IEnumerationStrategy enumerationStrategy, PictureSize size)
    {
        _enumerationStrategy = enumerationStrategy;
        Size = size;
        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public PictureSize Size { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (_buffer.Length < Size.PixelCount)
        {
            _buffer.Dispose();
            _buffer = DisposableArray<ColorTriplet>.OfSize(Size.PixelCount);
        }

        Span<ColorTriplet> span = picture.AsSpan();
        Span<ColorTriplet> bufferSpan = _buffer.AsSpan();

        var widthRatio = (float)(picture.Size.Width - 1) / Size.Width;
        var heightRatio = (float)(picture.Size.Height - 1) / Size.Height;

        foreach (var coordinate in _enumerationStrategy.Enumerate(Size))
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

                    first += localTriplet.First * Lanczos(xDiff - k) * Lanczos(yDiff - l);
                    second += localTriplet.Second * Lanczos(xDiff - k) * Lanczos(yDiff - l);
                    third += localTriplet.Third * Lanczos(xDiff - k) * Lanczos(yDiff - l);
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

    private double Lanczos(double x)
    {
        return x switch
        {
            0 => 1,
            > -3 and < 3 => 3 * Math.Sin(Math.PI * x) * Math.Sin(Math.PI * x / 3) / (Math.PI * Math.PI * x * x),
            _ => 0,
        };
    }
}