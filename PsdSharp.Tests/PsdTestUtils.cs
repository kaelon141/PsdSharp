using System.Buffers.Binary;
using System.Text;

namespace PsdSharp.Tests;

internal static class PsdTestUtils
{
    public static MemoryStream Ms(params byte[] bytes) => new(bytes, writable: false);

    public static byte[] BE16(ushort v)
    {
        var b = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(b, v);
        return b;
    }

    public static byte[] BE32(uint v)
    {
        var b = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(b, v);
        return b;
    }

    public static byte[] BE64(ulong v)
    {
        var b = new byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(b, v);
        return b;
    }

    public static byte[] Concat(params byte[][] arrays)
    {
        int len = 0;
        foreach (var a in arrays) len += a.Length;
        var result = new byte[len];
        int o = 0;
        foreach (var a in arrays)
        {
            Buffer.BlockCopy(a, 0, result, o, a.Length);
            o += a.Length;
        }
        return result;
    }

    public static byte[] Utf16Be(params ushort[] codeUnits)
    {
        var bytes = new byte[codeUnits.Length * 2];
        for (int i = 0; i < codeUnits.Length; i++)
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(i * 2, 2), codeUnits[i]);
        return bytes;
    }

    public static byte[] Pascal(string s, byte alignment = 2, Encoding? enc = null)
    {
        enc ??= Encoding.Latin1;
        var str = enc.GetBytes(s);
        if (str.Length > 255) throw new ArgumentOutOfRangeException(nameof(s), "Pascal string max length is 255");

        int total = 1 + str.Length;
        int rem = total % alignment;
        int pad = rem == 0 ? 0 : alignment - rem;

        var buf = new byte[1 + str.Length + pad];
        buf[0] = (byte)str.Length;
        Buffer.BlockCopy(str, 0, buf, 1, str.Length);
        // pad already zero-initialized
        return buf;
    }

    public static byte[] PascalNull(byte alignment = 2)
    {
        int pad = alignment - 1; // total=1, so pad to 2→1, to 4→3
        var buf = new byte[1 + pad];
        buf[0] = 0x00;
        return buf;
    }

    public static Stream GetAsset(string assetName)
    {
        var fullName = typeof(PsdTestUtils).Namespace + ".Assets." + assetName;
        var stream = typeof(PsdTestUtils).Assembly.GetManifestResourceStream(fullName);
        if (stream == null) throw new InvalidOperationException($"Could not find asset '{fullName}'");
        return stream;
    }
}