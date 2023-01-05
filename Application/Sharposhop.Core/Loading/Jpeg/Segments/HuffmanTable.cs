namespace Sharposhop.Core.Loading.Jpeg.Segments;

public class HuffmanTable
{
    public static readonly byte[] Marker = { 0xFF, 0xC4 };
    private readonly List<byte> _elements = new();
    private readonly List<object> _root = new();

    public HuffmanTable(byte[] data)
    {
        Length = data[0] << 8 | data[1];
        NumberOfTable = data[2] & 0x0F;
        Type = (TypeOfHuffmanTable)((data[2] & 0xF0) >> 4);
        NumbersOfCodes = data[3..19];
        var values = data[19..];
        var offset = 0;
        for (var i = 0; i < 16; i++)
        {
            var length = NumbersOfCodes[i];
            var codes = values[offset..(offset + length)];
            offset += length;
            _elements.AddRange(codes);
        }

        BuildTree();
    }

    public int Length { get; }
    public int NumberOfTable { get; }
    public TypeOfHuffmanTable Type { get; }
    public byte[] NumbersOfCodes { get; }

    public byte[] GetCode(int binaryAscii)
    {
        var res = Find(binaryAscii);
        if (res is byte[] bytes) return bytes;
        return Array.Empty<byte>();
    }

    private object Find(int binaryAscii)
    {
        var r = _root;
        while (r[binaryAscii] is List<object> leaf)
        {
            r = leaf;
        }

        return r;
    }

    private void BuildTree()
    {
        bool AddNode(object root, byte element, int pos)
        {
            if (root is not List<object> list) return false;
            if (pos == 0)
            {
                if (list.Count != 0) return false;
                list.Add(element);
                return true;
            }

            for (var i = 0; i < 2; i++)
            {
                if (list.Count == i) list.Add(new List<object>());
                if (AddNode(list[i], element, pos - 1)) return true;
            }

            return false;
        }

        var index = 0;
        for (var i = 0; i < 16; i++)
        {
            for (var j = 0; j < NumbersOfCodes[i]; j++)
            {
                AddNode(_root, _elements[index], i);
                index++;
            }
        }
    }
}

public enum TypeOfHuffmanTable
{
    AC,
    DC
}