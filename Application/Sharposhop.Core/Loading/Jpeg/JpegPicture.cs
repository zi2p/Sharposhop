using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading.Jpeg.Segments;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading.Jpeg;

public class JpegPicture
{
    private readonly Stream _data;

    public JpegPicture(
        ApplicationDefaultHeader header,
        QuantizationTable[] quantizationTables,
        StartOfFrame startOfFrame,
        HuffmanTable[] huffmanTables,
        StartOfScan startOfScan,
        Stream data)
    {
        Header = header ?? throw new ArgumentNullException(nameof(header));
        if (quantizationTables is null || quantizationTables.Length != 2)
            throw new ArgumentException("Quantization tables must be 2", nameof(quantizationTables));

        QuantizationTables = quantizationTables;
        StartOfFrame = startOfFrame ?? throw new ArgumentNullException(nameof(startOfFrame));
        if (huffmanTables is null || huffmanTables.Length > 4)
            throw new ArgumentException("Huffman tables must be 4", nameof(huffmanTables));

        HuffmanTables = huffmanTables;
        StartOfScan = startOfScan ?? throw new ArgumentNullException(nameof(startOfScan));
        data.Seek(0, SeekOrigin.Begin);
        _data = data;
    }

    public ApplicationDefaultHeader Header { get; }
    public QuantizationTable[] QuantizationTables { get; }
    public StartOfFrame StartOfFrame { get; }
    public HuffmanTable[] HuffmanTables { get; }
    public StartOfScan StartOfScan { get; }

    public PictureData DecodePicture(
        INormalizer normalizer,
        ISchemeConverterProvider schemeConverter,
        IEnumerationStrategy enumerationStrategy)
    {
        var firstCoefficient = 0;
        var secondCoefficient = 0;
        var thirdCoefficient = 0;

        var isGray = StartOfFrame.Components == 1;
        var firstIdcts = new List<InverseDiscreteCosineTransformation>();
        var secondIdcts = new List<InverseDiscreteCosineTransformation>();
        var thirdIdcts = new List<InverseDiscreteCosineTransformation>();
        PictureData picture;

        try
        {
            for (var y = 0; y < StartOfFrame.Height / 8; y++)
            {
                for (var x = 0; x < StartOfFrame.Width / 8; x++)
                {
                    var res1 = BuildMatrix(StartOfScan.Data, StartOfScan.ComponentSelectors[0].DcTable,
                        QuantizationTables.First(t =>
                            t.NumberOfTable == StartOfFrame.ComponentList[0].QuantizationTableId),
                        firstCoefficient);

                    firstCoefficient = res1.dc;
                    firstIdcts.Add(res1.idct);

                    if (isGray) continue;
                    var res2 = BuildMatrix(StartOfScan.Data, StartOfScan.ComponentSelectors[1].DcTable,
                        QuantizationTables.First(t =>
                            t.NumberOfTable == StartOfFrame.ComponentList[1].QuantizationTableId),
                        secondCoefficient);

                    var res3 = BuildMatrix(StartOfScan.Data, StartOfScan.ComponentSelectors[2].DcTable,
                        QuantizationTables.First(t =>
                            t.NumberOfTable == StartOfFrame.ComponentList[2].QuantizationTableId),
                        thirdCoefficient);

                    secondCoefficient = res2.dc;
                    secondIdcts.Add(res2.idct);
                    thirdCoefficient = res3.dc;
                    thirdIdcts.Add(res3.idct);
                }
            }

            picture = isGray ?
                GetPictureData(normalizer, schemeConverter, enumerationStrategy, firstIdcts, firstIdcts, firstIdcts) :
                GetPictureData(normalizer, schemeConverter, enumerationStrategy, firstIdcts, secondIdcts, thirdIdcts);
        }
        catch
        {
            throw new InvalidOperationException("Error during processing JPEG data");
        }

        return picture;
    }

    private PictureData GetPictureData(
        INormalizer normalizer,
        ISchemeConverterProvider schemeConverter,
        IEnumerationStrategy enumerationStrategy,
        List<InverseDiscreteCosineTransformation> firstIdcts,
        List<InverseDiscreteCosineTransformation> secondIdcts,
        List<InverseDiscreteCosineTransformation> thirdIdcts)
    {
        var isGray = StartOfFrame.Components == 1;
        var isRgb = StartOfFrame.ComponentList.First().Id == 'R';
        
        DisposableArray<ColorTriplet> data = DisposableArray<ColorTriplet>.OfSize(StartOfFrame.Width * StartOfFrame.Height);
        Span<ColorTriplet> dataSpan = data.AsSpan();

        var pictureSize = new PictureSize(StartOfFrame.Width, StartOfFrame.Height);

        foreach (var coordinate in enumerationStrategy.Enumerate(pictureSize))
        {
            var index = enumerationStrategy.AsContinuousIndex(coordinate, pictureSize);
            var first = firstIdcts[coordinate.X].BaseMatrix[coordinate.Y];
            var second = secondIdcts[coordinate.X].BaseMatrix[coordinate.Y];
            var third = thirdIdcts[coordinate.X].BaseMatrix[coordinate.Y];
            var (r, g, b) = isRgb ? ((byte)first, (byte)second, (byte)third) : ConvertColor(first, second, third);
            var triplet = new ColorTriplet(normalizer.Normalize(r), normalizer.Normalize(g), normalizer.Normalize(b));
            dataSpan[index] = schemeConverter.Converter.Revert(triplet);
        }

        return new PictureData(
            pictureSize,
            ColorScheme.Rgb,
            Gamma.DefaultGamma,
            data,
            !isGray);
    }

    private (byte, byte, byte) ConvertColor(double y, double cb, double cr)
    {
        var r = cr * (2 - 2 * .299) + y;
        var b = cb * (2 - 2 * .114) + y;
        var g = (y - .114 * b - .299 * r) / .587;
        return ((byte)Math.Clamp(r + 128, 0, 255),(byte) Math.Clamp(g + 128, 0, 255), (byte)Math.Clamp(b + 128, 0, 255));
    }

    private (InverseDiscreteCosineTransformation idct, int dc) BuildMatrix(
        BitData data,
        int index,
        QuantizationTable quantizationTable,
        int prevDcCoeff)
    {
        int Decode(int code, ulong bits)
        {
            var l = Math.Pow(2, code - 1);
            return (int) (bits >= l ? bits : bits - (ulong)(2 * l - 1));
        }

        var idct = new InverseDiscreteCosineTransformation();
        int code = HuffmanTables
            .First(t => t.Type == TypeOfHuffmanTable.DC && t.NumberOfTable == index)
            .GetCode(data);

        var bits = data.GetBitN(code);
        var dc = Decode(code, bits) + prevDcCoeff;
        idct.BaseMatrix[0] = dc * quantizationTable.Values[0];
        var c = 1;

        while (c < 64)
        {
            code = HuffmanTables
                .First(t => t.Type == TypeOfHuffmanTable.AC && t.NumberOfTable == index)
                .GetCode(data);

            if (code == 0)
                break;

            if (code > 15)
            {
                c += code >> 4;
                code &= 0x0F;
            }

            bits = data.GetBitN(code);

            if (c >= 64) continue;
            var coeff = Decode(code, bits);
            idct.BaseMatrix[c] = coeff * quantizationTable.Values[c];
            c += 1;
        }
        idct.PerformZigZag();
        idct.PerformIdct();

        return (idct, dc);
    }
}