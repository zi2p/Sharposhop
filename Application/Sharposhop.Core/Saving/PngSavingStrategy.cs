﻿using System.IO.Compression;
using System.Text;
using Sharposhop.Core.Extensions;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Loading.Png;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Saving;

public class PngSavingStrategy : ISavingStrategy
{
    private const byte DeflateConst = 120;
    private const int AdlerModulus = 65521;
    private const byte ChecksumConst = 1;
    private readonly INormalizer _normalizer;

    public PngSavingStrategy(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ValueTask SaveAsync(Stream stream, IPicture picture, Gamma gamma, bool isColored)
    {
        var pictureData = picture.AsSpan();
        const int bitDepth = 8;
        var colorType = isColored ? PngColor.Colored : PngColor.GrayScale;
        var imageData = Compress(pictureData, picture.Size, isColored);

        stream.Write(LoaderProxy.PngHeader, 0, LoaderProxy.PngHeader.Length);

        stream.WriteUInt32(13);
        var typeBytes = Encoding.ASCII.GetBytes("IHDR");
        var ihdrBytes = new byte[17];
        var width = Int32ToBytes(picture.Size.Width);
        var height = Int32ToBytes(picture.Size.Height);
        ihdrBytes[0] = typeBytes[0];
        ihdrBytes[1] = typeBytes[1];
        ihdrBytes[2] = typeBytes[2];
        ihdrBytes[3] = typeBytes[3];
        ihdrBytes[4] = width[0];
        ihdrBytes[5] = width[1];
        ihdrBytes[6] = width[2];
        ihdrBytes[7] = width[3];
        ihdrBytes[8] = height[0];
        ihdrBytes[9] = height[1];
        ihdrBytes[10] = height[2];
        ihdrBytes[11] = height[3];
        ihdrBytes[12] = bitDepth;
        ihdrBytes[13] = (byte)colorType;
        ihdrBytes[14] = (byte)CompressionMethod.DefaultCompression;
        ihdrBytes[15] = (byte)FilterMethod.DefaultFiltering;
        ihdrBytes[16] = (byte)InterlaceMethod.None;
        stream.Write(ihdrBytes, 0, 17);
        stream.WriteUInt32(CrcCalculator.CalculateCrc(ihdrBytes));

        if (gamma != Gamma.DefaultGamma)
        {
            stream.WriteUInt32(4);
            var gammaBytes = new byte[8];
            typeBytes = Encoding.ASCII.GetBytes("gAMA");
            typeBytes.CopyTo(gammaBytes, 0);
            var value = (int)(gamma.Reversed * 100000);
            var valueBytes = Int32ToBytes(value);
            valueBytes.CopyTo(gammaBytes, 4);
            stream.Write(gammaBytes, 0, 8);
            stream.WriteUInt32(CrcCalculator.CalculateCrc(gammaBytes));
        }

        stream.WriteUInt32((uint)imageData.LongLength);
        typeBytes = Encoding.ASCII.GetBytes("IDAT");
        stream.Write(typeBytes, 0, 4);
        stream.Write(imageData, 0, imageData.Length);
        stream.WriteUInt32(CrcCalculator.CalculateCrc(typeBytes, imageData));

        stream.WriteUInt32(0);
        typeBytes = Encoding.ASCII.GetBytes("IEND");
        stream.Write(typeBytes, 0, 4);
        stream.WriteUInt32(CrcCalculator.CalculateCrc(typeBytes));

        return ValueTask.CompletedTask;
    }
    
    private byte[] Compress(Span<ColorTriplet> pictureData, PictureSize size, bool isColored)
    {
        const int headerLength = 2;
        const int checksumLength = 4;
        var data = new byte[size.Width * size.Height * (isColored ? 3 : 1) + size.Height];
        var x = 0;
        var y = 0;

        foreach (var triplet in pictureData)
        {
            SetPixel(x++, y, data, triplet, size, isColored);
            if (x == size.Width)
            {
                x = 0;
                y++;
            }
        }

        using var compressStream = new MemoryStream();
        using var compressor = new DeflateStream(compressStream, CompressionLevel.Optimal, true);
        compressor.Write(data, 0, data.Length);
        compressor.Close();

        compressStream.Seek(0, SeekOrigin.Begin);

        var result = new byte[headerLength + compressStream.Length + checksumLength];

        result[0] = DeflateConst;
        result[1] = ChecksumConst;

        int streamValue;
        var i = 0;
        while ((streamValue = compressStream.ReadByte()) != -1)
        {
            result[headerLength + i] = (byte)streamValue;
            i++;
        }

        var checksum = CalculateAdlerChecksum(data);

        var offset = headerLength + compressStream.Length;
        result[offset++] = (byte)(checksum >> 24);
        result[offset++] = (byte)(checksum >> 16);
        result[offset++] = (byte)(checksum >> 8);
        result[offset] = (byte)(checksum >> 0);

        return result;
    }

    private static int CalculateAdlerChecksum(byte[] data)
    {
        var s1 = 1;
        var s2 = 0;

        var count = 0;
        foreach (var b in data)
        {
            if (data.Length > 0 && count == data.Length)
                break;

            s1 = (s1 + b) % AdlerModulus;
            s2 = (s1 + s2) % AdlerModulus;
            count++;
        }

        return s2 * 65536 + s1;
    }

    public void SetPixel(int x, int y, byte[] data, ColorTriplet triplet, PictureSize size,  bool isColored)
    {
        var rowStartPixel = 1 + y + (isColored ? 3 : 1) * size.Width * y;
        var pixelStartIndex = rowStartPixel + (isColored ? 3 : 1)  * x;

        if (isColored)
        {
            var first = _normalizer.DeNormalize(triplet.First);
            var second = _normalizer.DeNormalize(triplet.Second);
            var third = _normalizer.DeNormalize(triplet.Third);

            data[pixelStartIndex++] = first;
            data[pixelStartIndex++] = second;
            data[pixelStartIndex] = third;
        }
        else
        {
            var color = _normalizer.DeNormalize(triplet.First);
            data[pixelStartIndex] = color;
        }
    }

    private static byte[] Int32ToBytes(int value)
    {
        return new []
        {
            (byte) (value >> 24),
            (byte) (value >> 16),
            (byte) (value >> 8),
            (byte) value
        };
    }

    
}