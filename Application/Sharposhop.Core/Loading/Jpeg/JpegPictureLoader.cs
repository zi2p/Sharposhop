using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading.Jpeg.Segments;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading.Jpeg;

public class JpegPictureLoader : IPictureLoader
{
    private readonly byte[] _eofMarker = { 0xFF, 0xD9 };
    private readonly INormalizer _normalizer;
    private readonly ISchemeConverterProvider _schemeConverter;
    private readonly IEnumerationStrategy _enumerationStrategy;

    public JpegPictureLoader(
        INormalizer normalizer,
        ISchemeConverterProvider schemeConverterProvider,
        IEnumerationStrategy enumerationStrategy)
    {
        _normalizer = normalizer;
        _enumerationStrategy = enumerationStrategy;
        _schemeConverter = schemeConverterProvider;
    }

    public async ValueTask<PictureData> LoadImageAsync(Stream data)
    {
        ArgumentNullException.ThrowIfNull(data);
        await data.ReadAsync(new byte[2].AsMemory(0, 2));
        ApplicationDefaultHeader? header = null;
        var quantizationTables = new List<QuantizationTable>();
        StartOfFrame? startOfFrame = null;
        var huffmanTables = new List<HuffmanTable>();
        StartOfScan? startOfScan = null;

        while (true)
        {
            var marker = new byte[2];
            var lengthBytes = new byte[2];
            await data.ReadAsync(marker.AsMemory(0, 2));

            if (marker.SequenceEqual(_eofMarker))
                break;

            await data.ReadAsync(lengthBytes.AsMemory(0, 2));
            var length = lengthBytes[0] << 8 | lengthBytes[1];
            var block = new byte[length];
            data.Seek(-2, SeekOrigin.Current);
            await data.ReadAsync(block.AsMemory(0, length));

            if (marker.SequenceEqual(StartOfScan.Marker))
            {
                var bytes = new List<byte>();
                bytes.AddRange(block);
                var codedDataLenght = data.Length - data.Position - 2;
                var codedData = new byte[codedDataLenght];
                await data.ReadAsync(codedData.AsMemory(0, (int)codedDataLenght));
                bytes.AddRange(codedData);
                startOfScan = new StartOfScan(bytes.ToArray());
                continue;
            }

            if (marker.SequenceEqual(ApplicationDefaultHeader.Marker))
            {
                header = new ApplicationDefaultHeader(block);
                continue;
            }

            if (marker.SequenceEqual(QuantizationTable.Marker))
            {
                quantizationTables.Add(new QuantizationTable(block));
                continue;
            }

            if (marker.SequenceEqual(StartOfFrame.Marker))
            {
                startOfFrame = new StartOfFrame(block);
                continue;
            }

            if (marker.SequenceEqual(HuffmanTable.Marker))
            {
                huffmanTables.Add(new HuffmanTable(block));
            }
        }

        if (header is null)
            throw new InvalidOperationException("Application default header is not found");

        if (quantizationTables.Count == 0)
            throw new InvalidOperationException("Quantization tables are not found");

        if (startOfFrame is null)
            throw new InvalidOperationException("Start of frame is not found");

        if (huffmanTables.Count == 0)
            throw new InvalidOperationException("Huffman tables are not found");

        if (startOfScan is null)
            throw new InvalidOperationException("Start of scan is not found");

        var jpeg = new JpegPicture(header, quantizationTables.ToArray(), startOfFrame, huffmanTables.ToArray(), startOfScan);
        return new PictureData();
    }
}