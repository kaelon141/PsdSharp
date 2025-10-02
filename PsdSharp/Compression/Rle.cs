namespace PsdSharp.Compression;

internal static class Rle
{
    public static byte[] Decode(Stream stream, uint width, uint height, byte channelDepth, bool isPsb = false)
    {
        var rowSize = (int)(width * (channelDepth / 8));
        var result = new byte[rowSize * height];
        
        using var reader = new BinaryReader(stream);

        // 1. Read row lengths (big-endian 16-bit, PSD v1)
        var rowLengths = new int[height];
        for (var row = 0; row < height; row++)
        {
            if (isPsb)
            {
                // 32-bit big-endian
                rowLengths[row] = 
                    (reader.ReadByte() << 24) |
                    (reader.ReadByte() << 16) |
                    (reader.ReadByte() << 8) |
                    reader.ReadByte();
            }
            else
            {
                // 16-bit big-endian
                rowLengths[row] = 
                    (reader.ReadByte() << 8) |
                    reader.ReadByte();
            }
        }

        // 2. Decode each row
        var offset = 0;
        for (var row = 0; row < height; row++)
        {
            var compressedRow = reader.ReadBytes(rowLengths[row]);
            var decompressedRow = DecodePackBits(compressedRow, rowSize);

            if (decompressedRow.Length != rowSize)
                throw new InvalidDataException(
                    $"RLE row {row} expected {rowSize} bytes but got {decompressedRow.Length}.");

            Buffer.BlockCopy(decompressedRow, 0, result, offset, rowSize);
            offset += rowSize;
        }

        return result;
    }
    
    private static byte[] DecodePackBits(byte[] data, int expectedSize)
    {
        var result = new List<byte>(expectedSize);
        var i = 0;

        while (i < data.Length)
        {
            int control = (sbyte)data[i++]; // signed interpretation: -127..127, -128 = no-op
            if (control >= 0)
            {
                var count = control + 1;
                if (i + count > data.Length)
                    throw new InvalidDataException("PackBits literal overrun.");
                for (var j = 0; j < count; j++)
                    result.Add(data[i++]);
            }
            else if (control != -128)
            {
                var count = -control + 1;
                if (i >= data.Length)
                    throw new InvalidDataException("PackBits repeat missing value.");
                var value = data[i++];
                for (var j = 0; j < count; j++)
                    result.Add(value);
            }
            // 128 = no-op
        }

        if (result.Count != expectedSize)
            throw new InvalidDataException(
                $"PackBits expected {expectedSize} bytes but got {result.Count}.");

        return result.ToArray();
    }
}