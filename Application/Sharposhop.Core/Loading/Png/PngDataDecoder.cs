namespace Sharposhop.Core.Loading.Png;

public static class PngDataDecoder
{
    public static int GetBytesPerPixel(IhdrData imageHeader)
    {
        var bitDepthCorrected = imageHeader.BitDepth == 8 ? 1 :
            throw new NotSupportedException("Only 8 bit depth supported");
        var samplesPerPixel = GetPixelSize(imageHeader);
        return samplesPerPixel * bitDepthCorrected;
    }

    public static void Decode(byte[] imageBytes, IhdrData imageHeader, int bytesPerPixel)
    {
        var bytesPerImageLine = BytesPerImageLine(imageHeader, GetPixelSize(imageHeader));
        var currentRowStartByte = 1;

        for (var rowIndex = 0; rowIndex < imageHeader.Height; rowIndex++)
        {
            var filterType = (FilterType)imageBytes[currentRowStartByte - 1];
            var previousRowStartByte = rowIndex + bytesPerImageLine * (rowIndex - 1);
            var end = currentRowStartByte + bytesPerImageLine;

            for (var currentByteAbsolute = currentRowStartByte; currentByteAbsolute < end; currentByteAbsolute++)
            {
                ReadWithFilter(imageBytes, filterType, previousRowStartByte, currentRowStartByte,
                    currentByteAbsolute, currentByteAbsolute - currentRowStartByte, bytesPerPixel);
            }

            currentRowStartByte += bytesPerImageLine + 1;
        }
    }

    private static void ReadWithFilter(
        byte[] bytes,
        FilterType filterType,
        int previousRowStartByte,
        int rowStartByte,
        int currentByte,
        int rowByteIndex,
        int bytesPerPixel)
    {
        byte GetLeftByteValue()
        {
            var leftIndex = rowByteIndex - bytesPerPixel;
            var leftValue = leftIndex >= 0 ? bytes[rowStartByte + leftIndex] : (byte)0;
            return leftValue;
        }

        byte GetAboveByteValue()
        {
            var upIndex = previousRowStartByte + rowByteIndex;
            return upIndex >= 0 ? bytes[upIndex] : (byte)0;
        }

        byte GetAboveLeftByteValue()
        {
            var index = previousRowStartByte + rowByteIndex - bytesPerPixel;
            return index < previousRowStartByte || previousRowStartByte < 0 ? (byte)0 : bytes[index];
        }

        switch (filterType)
        {
            case FilterType.None: return;
            case FilterType.Up:
            {
                var above = previousRowStartByte + rowByteIndex;
                if (above < 0 || above >= bytes.Length)
                    return;

                bytes[currentByte] += bytes[above];
                return;
            }
            case FilterType.Sub:
            {
                var leftIndex = rowByteIndex - bytesPerPixel;
                if (leftIndex < 0 || rowStartByte + leftIndex >= bytes.Length)
                    return;

                bytes[currentByte] += bytes[rowStartByte + leftIndex];
                return;
            }
            case FilterType.Average:
                bytes[currentByte] += (byte) ((GetLeftByteValue() + GetAboveByteValue()) / 2);
                break;
            case FilterType.Paeth:
                var a = GetLeftByteValue();
                var b = GetAboveByteValue();
                var c = GetAboveLeftByteValue();
                bytes[currentByte] += GetPaethValue(a, b, c);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Encountered unknown filter");
        }
    }

    private static int GetPixelSize(IhdrData imageHeader)
    {
        return imageHeader.Color switch
        {
            PngColor.GrayScale => 1,
            PngColor.Palette => 1,
            PngColor.Colored => 3,
            _ => 0
        };
    }

    private static int BytesPerImageLine(IhdrData imageHeader, int samplesPerPixel)
    {
        return imageHeader.BitDepth switch
        {
            8 => imageHeader.Width * samplesPerPixel,
            _ => throw new NotSupportedException("Only 8 bit per channel supported"),
        };
    }

    private static byte GetPaethValue(byte a, byte b, byte c)
    {
        var p = a + b - c;
        var pa = Math.Abs(p - a);
        var pb = Math.Abs(p - b);
        var pc = Math.Abs(p - c);

        if (pa <= pb && pa <= pc)
            return a;

        return pb <= pc ? b : c;
    }

    public enum FilterType
    {
        None = 0,
        Sub = 1,
        Up = 2,
        Average = 3,
        Paeth = 4
    }
}