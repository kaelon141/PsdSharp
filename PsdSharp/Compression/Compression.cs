using System.IO.Compression;

namespace PsdSharp.Compression;

internal static class Compression
{
    public static byte[] Decompress(byte[] data, ImageCompression compression, uint width, uint height, byte channelDepth, bool isPsb = false)
        => Decompress(new MemoryStream(data), compression, width, height, channelDepth, isPsb);
    public static byte[] Decompress(Stream stream, ImageCompression compression, uint width, uint height, byte channelDepth, bool isPsb = false)
    {
        var decompressStream = new MemoryStream();
        switch (compression)
        {
            case ImageCompression.Raw or ImageCompression.Rle:
                stream.CopyTo(decompressStream);
                break;
            case ImageCompression.ZipWithoutPrediction or ImageCompression.ZipWithPrediction:
            {
                using var deflate = new ZLibStream(stream, CompressionMode.Decompress, leaveOpen: true);
                deflate.CopyTo(decompressStream);
                break;
            }
        }
        decompressStream.Position = 0;
        
        switch (compression)
        {
            case ImageCompression.Raw:
                return decompressStream.ToArray();
            case ImageCompression.Rle:
                return Rle.Decode(decompressStream, width, height, channelDepth, isPsb);
            case ImageCompression.ZipWithoutPrediction:
                return decompressStream.ToArray();
            case ImageCompression.ZipWithPrediction:
            default:
            {
                return DecodePrediction(decompressStream.ToArray(), width, height, channelDepth);
            }
        }
    }

    private static byte[] DecodePrediction(byte[] data, uint width, uint height, byte depth)
    {
        if (depth == 8)
        {
            var arr = (byte[])data.Clone();
            DeltaDecode(arr, 256, width, height);
            return arr;
        }

        if (depth == 16)
        {
            // Interpret as big-endian ushort[]
            var arr = new ushort[width * height];
            for (var i = 0; i < arr.Length; i++)
                arr[i] = (ushort)((data[2 * i] << 8) | data[2 * i + 1]);

            DeltaDecode(arr, 65536, width, height);

            // Write back as big-endian bytes
            var result = new byte[arr.Length * 2];
            for (var i = 0; i < arr.Length; i++)
            {
                result[2 * i] = (byte)(arr[i] >> 8);
                result[2 * i + 1] = (byte)(arr[i] & 0xFF);
            }
            return result;
        }

        if (depth == 32)
        {
            // Treat as bytes first
            var arr = (byte[])data.Clone();
            DeltaDecode(arr, 256, width * 4, height);
            arr = RestoreByteOrder(arr, width, height);
            return arr;
        }

        throw new ArgumentException($"Unsupported depth {depth}");
    }
    
    private static void DeltaDecode(byte[] arr, int mod, uint w, uint h)
    {
        for (var y = 0; y < h; y++)
        {
            var offset = y * w;
            for (var x = 0; x < w - 1; x++)
            {
                var pos = offset + x;
                arr[pos + 1] = (byte)((arr[pos + 1] + arr[pos]) % mod);
            }
        }
    }

    private static void DeltaDecode(ushort[] arr, int mod, uint w, uint h)
    {
        for (var y = 0; y < h; y++)
        {
            var offset = y * w;
            for (var x = 0; x < w - 1; x++)
            {
                var pos = offset + x;
                arr[pos + 1] = (ushort)((arr[pos + 1] + arr[pos]) % mod);
            }
        }

        // byteswap back to big endian
        for (var i = 0; i < arr.Length; i++)
        {
            var v = arr[i];
            arr[i] = (ushort)((v >> 8) | (v << 8));
        }
    }
    
    private static byte[] RestoreByteOrder(byte[] arr, uint w, uint h)
    {
        var result = new byte[arr.Length];
        var i = 0;
        var rowSize = (int)(4 * w);

        for (var row = 0; row < rowSize * h; row += rowSize)
        {
            for (var offset = row; offset < row + w; offset++)
            {
                for (var x = offset; x < offset + rowSize; x += (int)w)
                {
                    result[i++] = arr[x];
                }
            }
        }

        return result;
    }
}