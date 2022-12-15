using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class ContrastAdaptiveSharpening : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly float _sharpness;
    private const int Radius = 5;

    public ContrastAdaptiveSharpening(float sharpness, IEnumerationStrategy enumerationStrategy)
    {
        _sharpness = sharpness;
        _enumerationStrategy = enumerationStrategy;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        foreach (var coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var x = coordinate.X;
            var y = coordinate.Y;
            if (x - Radius < 0 || x + Radius >= picture.Size.Width || y - Radius < 0 ||
                y + Radius >= picture.Size.Height)
                continue;
            Span<ColorTriplet> span = picture.AsSpan();

            var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
            var maxFirst = span[index].First;
            var minFirst = span[index].First;
            var maxSecond = span[index].Second;
            var minSecond = span[index].Second;
            var maxThird = span[index].Third;
            var minThird = span[index].Third;

            float first, second, third;

            for (var xx = x - Radius; xx <= x + Radius; xx++)
            {
                for (var yy = y - Radius; yy <= y + Radius; yy++)
                {
                    if (span[xx + yy * picture.Size.Width].First > maxFirst)
                        maxFirst = span[xx + yy * picture.Size.Width].First;

                    else if (span[xx + yy * picture.Size.Width].First < minFirst)
                        minFirst = span[xx + yy * picture.Size.Width].First;

                    if (span[xx + yy * picture.Size.Width].Second > maxSecond)
                        maxSecond = span[xx + yy * picture.Size.Width].Second;

                    else if (span[xx + yy * picture.Size.Width].Second < minSecond)
                        minSecond = span[xx + yy * picture.Size.Width].Second;

                    if (span[xx + yy * picture.Size.Width].Third > maxThird)
                        maxThird = span[xx + yy * picture.Size.Width].Third;

                    else if (span[xx + yy * picture.Size.Width].Third < minThird)
                        minThird = span[xx + yy * picture.Size.Width].Third;
                }
            }

            if (span[index].First + _sharpness > maxFirst)
            {
                first = maxFirst + Alpha(maxFirst - minFirst) * (span[index].First + _sharpness - maxFirst);
            }
            else if (span[index].First + _sharpness < minFirst)
            {
                first = minFirst - Alpha(maxFirst - minFirst) * (-span[index].First - _sharpness + minFirst);
            }
            else
            {
                first = span[index].First + _sharpness;
            }

            if (span[index].Second + _sharpness > maxSecond)
            {
                second = maxSecond + Alpha(maxSecond - minSecond) * (span[index].Second + _sharpness - maxSecond);
            }
            else if (span[index].Second + _sharpness < minSecond)
            {
                second = minSecond - Alpha(maxSecond - minSecond) * (-span[index].Second - _sharpness + minSecond);
            }
            else
            {
                second = span[index].Second + _sharpness;
            }

            if (span[index].Third + _sharpness > maxThird)
            {
                third = maxThird + Alpha(maxThird - minThird) * (span[index].Third + _sharpness - maxThird);
            }
            else if (span[index].Third + _sharpness < minThird)
            {
                third = minThird - Alpha(maxThird - minThird) * (-span[index].Third - _sharpness + minThird);
            }
            else
            {
                third = span[index].Third + _sharpness;
            }

            span[index] = new ColorTriplet(first, second, third);
        }

        return new ValueTask<IPicture>(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    private float Alpha(float strength)
    {
        if (strength == 0)
            return 0;
        return 1f / (1.0f / 32.0f + strength);
    }
}