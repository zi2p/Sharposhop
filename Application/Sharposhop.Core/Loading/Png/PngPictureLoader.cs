using System.IO.Compression;
using System.Text;
using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Extensions;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading.Png;

public class PngPictureLoader : IPictureLoader
{
    private readonly INormalizer _normalizer;
    private readonly ISchemeConverterProvider _schemeConverter;
    private readonly IEnumerationStrategy _enumerationStrategy;

    public PngPictureLoader(
        INormalizer normalizer,
        ISchemeConverterProvider schemeConverter,
        IEnumerationStrategy enumerationStrategy)
    {
        _normalizer = normalizer;
        _enumerationStrategy = enumerationStrategy;
        _schemeConverter = schemeConverter;
    }

    public async ValueTask<PictureData> LoadImageAsync(Stream data)
    {
        var png = await Open(data);
        return png.GetPictureData(_normalizer, _enumerationStrategy, _schemeConverter);
    }

    private async ValueTask<PngImage> Open(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _ = await stream.ReadAsync(new byte[8], 0, 8);
        var crc = new byte[4];
        var imageHeader = await ReadImageHeaderAsync(stream, crc);
        var isNotEnd = true;
        var gamma = Gamma.DefaultGamma;
        Palette? palette = null;

        await using var output = new MemoryStream();
        await using var memoryStream = new MemoryStream();
        while (isNotEnd)
        {
            var headerNew = await ReadChunkHeaderAsync(stream);
            if (!headerNew.HasValue) break;
            var header = headerNew.Value;
            var bytes = new byte[header.Length];
            var read = await stream.ReadAsync(bytes);

            if (read != bytes.Length)
                throw new InvalidOperationException(
                    $"Cannot read {header.Length} bytes for the {header} header, only found. Ong is incorrect");

            switch (header.Name)
            {
                case "PLTE":
                    if (header.Length % 3 != 0)
                        throw new InvalidOperationException("Palette lenght must be multiple of 3.");

                    if (imageHeader.Color.HasFlag(PngColor.Palette))
                        palette = new Palette(bytes);
                    break;
                case "IDAT":
                    memoryStream.Write(bytes, 0, bytes.Length);
                    break;
                case "IEND":
                    isNotEnd = false;
                    break;
                case "tRNS":
                    throw new NotSupportedException("Transparency is not supported");
                case "gAMA":
                {
                    var value = GetInt32FromBytes(bytes, 0);
                    gamma = new Gamma(1f / (value / 100000f));
                    break;
                }
                default:
                    if (header.IsCritical) throw new NotSupportedException($"{header} critical header is not supported.");
                    break;
            }

            read = await stream.ReadAsync(crc);
            if (read != 4)
            {
                throw new InvalidOperationException("CRC was read incorrectly.");
            }
        }

        await memoryStream.FlushAsync();
        memoryStream.Seek(2, SeekOrigin.Begin);
        await using var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress);
        await deflateStream.CopyToAsync(output);
        deflateStream.Close();

        var imageBytes = output.ToArray();
        var bytesPerPixel = PngDataDecoder.GetBytesPerPixel(imageHeader);
        PngDataDecoder.Decode(imageBytes, imageHeader, bytesPerPixel);
        return new PngImage(imageHeader, new PngData(imageBytes, bytesPerPixel, imageHeader, palette), gamma);
    }

    private async ValueTask<ChunkHeader?> ReadChunkHeaderAsync(Stream stream)
    {
        ChunkHeader? chunkHeader = null;

        var position = stream.Position;
        var headerBytes = await stream.ReadPngHeaderBytesAsync();

        if (headerBytes.Length == 0)
            return chunkHeader;

        var length = GetInt32FromBytes(headerBytes, 0);
        var name = Encoding.ASCII.GetString(headerBytes, 4, 4);
        chunkHeader = new ChunkHeader(position, length, name);
        return chunkHeader;
    }

    private async ValueTask<IhdrData> ReadImageHeaderAsync(Stream stream, byte[] crc)
    {
        var headerNew = await ReadChunkHeaderAsync(stream);
        if (!headerNew.HasValue)
            throw new ArgumentException("No chunks found");

        var header = headerNew.Value;

        if (header.Name != "IHDR")
            throw new ArgumentException($"The first chunk must be IHDR chunk. Got {header}.");

        if (header.Length != 13)
            throw new ArgumentException($"The IHDR chunk must be 13 bytes long.");

        var ihdrBytes = new byte[13];
        await stream.ReadAsync(ihdrBytes);
        await stream.ReadAsync(crc);

        var width = GetInt32FromBytes(ihdrBytes, 0);
        var height = GetInt32FromBytes(ihdrBytes, 4);
        var bitDepth = ihdrBytes[8];
        var colorType = ihdrBytes[9];
        var compressionMethod = ihdrBytes[10];
        var filterMethod = ihdrBytes[11];
        var interlaceMethod = ihdrBytes[12];

        return new IhdrData(width, height, bitDepth, (PngColor)colorType,
            (CompressionMethod)compressionMethod, (FilterMethod)filterMethod,
            (InterlaceMethod)interlaceMethod);
    }

    private static int GetInt32FromBytes(byte[] bytes, int offset)
    {
        return (bytes[0 + offset] << 24) + (bytes[1 + offset] << 16) + (bytes[2 + offset] << 8) + bytes[3 + offset];
    }
}

        