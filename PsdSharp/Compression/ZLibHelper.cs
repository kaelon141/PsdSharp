namespace PsdSharp.Compression;

#if NET6_0_OR_GREATER
using System.IO.Compression;

internal static class ZLibHelper
{
    public static void Decompress(Stream input, Stream output)
    {
        using var zlib = new ZLibStream(input, CompressionMode.Decompress, leaveOpen: true);
        zlib.CopyTo(output);
    }
}

#else
using Ionic.Zlib;

internal static class ZLibHelper
{
    public static void Decompress(Stream input, Stream output)
    {
        using var zlib = new ZlibStream(input, CompressionMode.Decompress, leaveOpen: true);
        zlib.CopyTo(output);
    }
}
#endif