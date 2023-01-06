namespace Sharposhop.Core.Loading.Jpeg.Segments;

public class StartOfFrame
{
    public static readonly byte[] Marker = { 0xFF, 0xC0 };
    private readonly List<Component> _components = new ();

    public StartOfFrame(byte[] data)
    {
        Length = data[0] << 8 | data[1];
        Precision = data[2];
        Height = data[3] << 8 | data[4];
        Width = data[5] << 8 | data[6];
        Components = data[7];

        for (var i = 0; i < Components; i++)
        {
            _components.Add(new Component(data[8 + i * 3], data[9 + i * 3], data[10 + i * 3]));
        }

        if (Components is not (1 or 3))
        {
            throw new NotSupportedException("Only 1 (grayscale) or 3 (colored) number of components are supported");
        }
    }

    public int Length { get; }
    public int Precision { get; }
    public int Height { get; }
    public int Width { get; }
    public int Components { get; }
    public IReadOnlyList<Component> ComponentList => _components;
    
    public record struct Component (byte Id, byte SamplingFactor, byte QuantizationTableId);
}