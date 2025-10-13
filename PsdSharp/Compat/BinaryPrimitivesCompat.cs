#if !NET6_0_OR_GREATER
using System.Runtime.InteropServices;

namespace PsdSharp.Compat;

public static class BinaryPrimitivesCompat
{
    public static float ReadSingleBigEndian(ReadOnlySpan<byte> source)
    {
        if (source.Length < 4)
            throw new ArgumentOutOfRangeException(nameof(source), "Need at least 4 bytes to read a Single.");

        int bits =
            (source[0] << 24) |
            (source[1] << 16) |
            (source[2] << 8) |
            (source[3]);

        // reinterpret the int bits as a float
        return Int32BitsToSingle(bits);
    }
    
    public static double ReadDoubleBigEndian(ReadOnlySpan<byte> source)
    {
        if (source.Length < 8)
            throw new ArgumentOutOfRangeException(nameof(source), "Need at least 8 bytes to read a Double.");
        
        // Read as 64-bit integer in big-endian order
        var value =
            ((long)source[0] << 56) |
            ((long)source[1] << 48) |
            ((long)source[2] << 40) |
            ((long)source[3] << 32) |
            ((long)source[4] << 24) |
            ((long)source[5] << 16) |
            ((long)source[6] << 8) |
            ((long)source[7]);

        return BitConverter.Int64BitsToDouble(value);
    }
    
    private static float Int32BitsToSingle(int value)
    {
        Span<int> intSpan = stackalloc int[1] { value };
        return MemoryMarshal.Cast<int, float>(intSpan)[0];
    }
}

#endif
