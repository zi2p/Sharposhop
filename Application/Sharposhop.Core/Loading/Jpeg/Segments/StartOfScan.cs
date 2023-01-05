namespace Sharposhop.Core.Loading.Jpeg.Segments;

public class StartOfScan
{
    public static readonly byte[] Marker = { 0xFF, 0xDA };
    private readonly List<ComponentSelector> _componentSelectors = new ();

    public StartOfScan(byte[] data)
    {
        Length = data[0] << 8 | data[1];
        NumberOfComponents = data[2];
        for (var i = 0; i < NumberOfComponents; i++)
        {
            _componentSelectors.Add(new ComponentSelector(data[3 + i * 2], data[4 + i * 2] >> 4, data[4 + i * 2] & 0x0F));
        }

        StartOfSpectralSelection = data[3 + NumberOfComponents * 2];
        EndOfSpectralSelection = data[4 + NumberOfComponents * 2];
        SuccessiveApproximation = data[5 + NumberOfComponents * 2];
        var rawData = data[(6 + NumberOfComponents * 2)..];
        var trimmedData = new List<byte>();
        for (var i = 0; i < rawData.Length - 1; i++)
        {
            if (rawData[i] == 0xFF && rawData[i + 1] == 0x00)
            {
                trimmedData.Add(rawData[i]);
                i++;
            }
            else
            {
                trimmedData.Add(rawData[i]);
            }
        }
        Data = trimmedData.ToArray();
    }
    
    public int Length { get; }
    public int NumberOfComponents { get; }
    public IReadOnlyCollection<ComponentSelector> ComponentSelectors => _componentSelectors;
    public int StartOfSpectralSelection { get; }
    public int EndOfSpectralSelection { get; }
    public int SuccessiveApproximation { get; }
    public byte[] Data { get; }

    public record struct ComponentSelector(int ComponentId, int DcTable, int AcTable);
}