using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class ContrastAdaptiveSharpening : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private const int Radius = 5;

    public ContrastAdaptiveSharpening(float sharpness, IEnumerationStrategy enumerationStrategy)
    {
        Sharpness = sharpness;
        _enumerationStrategy = enumerationStrategy;
    }

    private float Sharpness { get; }

    public async ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        await Parallel.ForEachAsync(_enumerationStrategy.Enumerate(picture.Size).AsEnumerable(), (coordinate, _) =>
        {
            ProcessCoordinate(picture, coordinate);
            return ValueTask.CompletedTask;
        });

        return picture;
    }

    private void ProcessCoordinate(IPicture picture, PlaneCoordinate coordinate)
    {
        var x = coordinate.X;
        var y = coordinate.Y;
        if (x - Radius < 0 || x + Radius >= picture.Size.Width || y - Radius < 0 ||
            y + Radius >= picture.Size.Height)
            return;
        Span<ColorTriplet> span = picture.AsSpan();


        var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);

        var input = span[index];

        var maxFirst = span[index].First;
        var minFirst = span[index].First;
        var maxSecond = span[index].Second;
        var minSecond = span[index].Second;
        var maxThird = span[index].Third;
        var minThird = span[index].Third;

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

        var strengthFirst = Math.Max(minFirst, 1 - maxFirst);
        var strengthSecond = Math.Max(minSecond, 1 - maxSecond);
        var strengthThird = Math.Max(minThird, 1 - maxThird);


        float first, second, third;

        if (span[index].First + Sharpness > maxFirst)
        {
            first = maxFirst + Alpha(strengthFirst) * (span[index].First + Sharpness - maxFirst);
        }
        else if (span[index].First + Sharpness < minFirst)
        {
            first = minFirst - Alpha(strengthFirst) * (-span[index].First - Sharpness + minFirst);
        }
        else
        {
            first = span[index].First + Sharpness;
        }

        if (span[index].Second + Sharpness > maxSecond)
        {
            second = maxSecond + Alpha(strengthSecond) * (span[index].Second + Sharpness - maxSecond);
        }
        else if (span[index].Second + Sharpness < minSecond)
        {
            second = minSecond - Alpha(strengthSecond) * (-span[index].Second - Sharpness + minSecond);
        }
        else
        {
            second = span[index].Second + Sharpness;
        }

        if (span[index].Third + Sharpness > maxThird)
        {
            third = maxThird + Alpha(strengthThird) * (span[index].Third + Sharpness - maxThird);
        }
        else if (span[index].Third + Sharpness < minThird)
        {
            third = minThird - Alpha(strengthThird) * (-span[index].Third - Sharpness + minThird);
        }
        else
        {
            third = span[index].Third + Sharpness;
        }


        var triplet = new ColorTriplet(first, second, third);

        if (triplet.Equals(new ColorTriplet(1, 1, 1)))
        {
            Console.WriteLine();
        }

        span[index] = triplet;
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    private float Alpha(float strength)
    {
        return 1f / (1.0f / 32.0f + strength);
    }
}