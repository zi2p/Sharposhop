using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.Saving.Tools;

public readonly struct StreamTripletWriter : ITripletWriter
{
    private readonly Stream _stream;
    private readonly INormalizer _normalizer;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly int _width;
    private readonly int _height;
    private readonly int _streamOffset;

    public StreamTripletWriter(
        Stream stream,
        int width,
        int height,
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy,
        int streamOffset)
    {
        _stream = stream;
        _normalizer = normalizer;
        _enumerationStrategy = enumerationStrategy;
        _streamOffset = streamOffset;
        _width = width;
        _height = height;
    }

    public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
    {
        var first = _normalizer.DeNormalize(triplet.First);
        var second = _normalizer.DeNormalize(triplet.Second);
        var third = _normalizer.DeNormalize(triplet.Third);

        var index = _enumerationStrategy.AsContinuousIndex(coordinate, _width, _height) * 3;

        if (_stream.Position.Equals(_streamOffset + index) is false)
            _stream.Position = _streamOffset + index;

        _stream.WriteByte(first);
        _stream.WriteByte(second);
        _stream.WriteByte(third);

        return ValueTask.CompletedTask;
    }
}