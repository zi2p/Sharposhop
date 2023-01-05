using System.Text;
using Sharposhop.Core.Loading.Jpeg.Segments;

namespace Sharposhop.Core.Loading.Jpeg;

public class JpegPicture
{
    public JpegPicture(
        ApplicationDefaultHeader header,
        QuantizationTable[] quantizationTables,
        StartOfFrame startOfFrame,
        HuffmanTable[] huffmanTables,
        StartOfScan startOfScan)
    {
        Header = header ?? throw new ArgumentNullException(nameof(header));
        if (quantizationTables is not null && quantizationTables.Length != 2)
            throw new ArgumentException("Quantization tables must be 2", nameof(quantizationTables));

        QuantizationTables = quantizationTables;
        StartOfFrame = startOfFrame ?? throw new ArgumentNullException(nameof(startOfFrame));
        if (huffmanTables is not null && huffmanTables.Length > 4)
            throw new ArgumentException("Huffman tables must be 4", nameof(huffmanTables));

        HuffmanTables = huffmanTables;
        StartOfScan = startOfScan ?? throw new ArgumentNullException(nameof(startOfScan));
    }

    public ApplicationDefaultHeader Header { get; }
    public QuantizationTable[] QuantizationTables { get; }
    public StartOfFrame StartOfFrame { get; }
    public HuffmanTable[] HuffmanTables { get; }
    public StartOfScan StartOfScan { get; }

    // private void BuildMatrix(int id, int index, int componentNumber)
    // {
    //     var idct = new InverseDiscreteCosineTransformation();
    //
    //     var code = HuffmanTables
    //         .FirstOrDefault(t => t.Type == TypeOfHuffmanTable.DC && t.NumberOfTable == id)!
    //         .GetCode(StartOfScan.Data[index]);
    //
    //     bits = st.GetBitN(code)
    //     dccoeff = DecodeNumber(code, bits) + olddccoeff
    //
    //     i.base[0] = (dccoeff) * quant[0]
    //     l = 1
    //     while l < 64:
    //     code = self.huffman_tables[16 + idx].GetCode(st)
    //     if code == 0:
    //     break
    //
    //
    //     if code > 15:
    //     l += code >> 4
    //     code = code & 0x0F
    //
    //     bits = st.GetBitN(code)
    //
    //     if l < 64:
    //     coeff = DecodeNumber(code, bits)
    //     i.base[l] = coeff * quant[l]
    //     l += 1
    //
    //     i.rearrange_using_zigzag()
    //     i.perform_IDCT()
    //
    //     return i, dccoeff
    // }
    //
    // private int GetBit(int index)
    // {
    //     var b = StartOfScan.Data[index]
}