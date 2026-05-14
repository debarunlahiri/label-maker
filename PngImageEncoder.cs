using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using ZXing.Rendering;

namespace LabelMaker;

internal static class PngImageEncoder
{
    private static readonly uint[] CrcTable = CreateCrcTable();

    public static byte[] Encode(PixelData pixelData)
    {
        using var output = new MemoryStream();

        output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        WriteHeader(output, pixelData.Width, pixelData.Height);
        WriteImageData(output, pixelData);
        WriteChunk(output, "IEND", Array.Empty<byte>());

        return output.ToArray();
    }

    private static void WriteHeader(Stream output, int width, int height)
    {
        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header[0..4], width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..8], height);
        header[8] = 8; // Bit depth
        header[9] = 6; // RGBA
        header[10] = 0; // Deflate
        header[11] = 0; // Standard filter
        header[12] = 0; // No interlace

        WriteChunk(output, "IHDR", header.ToArray());
    }

    private static void WriteImageData(Stream output, PixelData pixelData)
    {
        var source = pixelData.Pixels;
        var stride = pixelData.Width * 4;
        var raw = new byte[(stride + 1) * pixelData.Height];

        for (var y = 0; y < pixelData.Height; y++)
        {
            var rawOffset = y * (stride + 1);
            var sourceOffset = y * stride;
            raw[rawOffset] = 0; // No per-row PNG filter.

            for (var x = 0; x < pixelData.Width; x++)
            {
                var sourcePixel = sourceOffset + x * 4;
                var targetPixel = rawOffset + 1 + x * 4;

                raw[targetPixel] = source[sourcePixel + 2];
                raw[targetPixel + 1] = source[sourcePixel + 1];
                raw[targetPixel + 2] = source[sourcePixel];
                raw[targetPixel + 3] = source[sourcePixel + 3];
            }
        }

        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.Fastest, leaveOpen: true))
        {
            zlib.Write(raw, 0, raw.Length);
        }

        WriteChunk(output, "IDAT", compressed.ToArray());
    }

    private static void WriteChunk(Stream output, string type, byte[] data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        output.Write(length);

        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        var crc = CalculateCrc(typeBytes, data);
        Span<byte> crcBytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crcBytes, crc);
        output.Write(crcBytes);
    }

    private static uint CalculateCrc(byte[] type, byte[] data)
    {
        var crc = 0xffffffffu;

        foreach (var value in type)
            crc = CrcTable[(crc ^ value) & 0xff] ^ (crc >> 8);

        foreach (var value in data)
            crc = CrcTable[(crc ^ value) & 0xff] ^ (crc >> 8);

        return crc ^ 0xffffffffu;
    }

    private static uint[] CreateCrcTable()
    {
        var table = new uint[256];

        for (uint i = 0; i < table.Length; i++)
        {
            var value = i;

            for (var bit = 0; bit < 8; bit++)
            {
                value = (value & 1) == 1
                    ? 0xedb88320u ^ (value >> 1)
                    : value >> 1;
            }

            table[i] = value;
        }

        return table;
    }
}
