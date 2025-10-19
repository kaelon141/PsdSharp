using System.Buffers.Binary;
using PsdSharp.Parsing;

namespace PsdSharp;

public enum ColorSpaceId : ushort
{
    Rgb = 0,
    Hsb = 1,
    Cmyk = 2,
    Lab = 7,
    Grayscale = 8
}

public struct PsdColor
{
    public ColorSpaceId ColorSpace { get; set; }
    public byte[] Data { get; set; }

    public PsdColor(ColorSpaceId colorSpace, byte[] data)
    {
        ColorSpace = colorSpace;
        Data = data;
    }

    public static PsdColor FromRgb(ushort r, ushort g, ushort b)
    { 
        var data = new byte[10];
        BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(0, 2), r);
        BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(2, 2), g);
        BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(4, 2), b);
        return new PsdColor(ColorSpaceId.Rgb, data);
    }

    public static PsdColor FromRgb(byte r, byte g, byte b)
        => FromRgb((ushort)r, g, b);

    internal static PsdColor FromPhotoshopData(BigEndianReader reader)
    {
        var colorSpace = (ColorSpaceId)reader.ReadUInt16();
        var buffer = new byte[8];
        reader.ReadIntoBuffer(buffer);
        return new PsdColor(colorSpace, buffer);
    }

    public (ushort Red, ushort Green, ushort Blue) ToRgb()
    {
        switch (ColorSpace)
        {
            case ColorSpaceId.Rgb:
                var red = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(0, 2));
                var green = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(2, 2));
                var blue = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(4, 2));
                return (red, green, blue);
            case ColorSpaceId.Hsb:
                var h = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(0, 2)) / 65_535.0f;
                var s = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(2, 2)) / 65_535.0f;
                var v = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(4, 2)) / 65_535.0f;
                return HsbToRgb(h, s, v);
            case ColorSpaceId.Cmyk:
                var c = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(0, 2));
                var m = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(2, 2));
                var y = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(4, 2));
                var k = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(6, 2));
                return CmykToRgb(c, m, y, k);
            case ColorSpaceId.Lab:
                var l = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(0, 2));
                var a = BinaryPrimitives.ReadInt16BigEndian(Data.AsSpan(2, 2));
                var b = BinaryPrimitives.ReadInt16BigEndian(Data.AsSpan(4, 2));
                return LabToRgb(l, a, b);
            case ColorSpaceId.Grayscale:
                var g = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(0, 2)) / 10_000f;
                var rgb = (ushort)Math.Round(Compat.MathCompat.Clamp(g, 0f, 1f) * ushort.MaxValue);
                return (rgb, rgb, rgb);
            default: return (0, 0, 0);
        }
    }

    private static (ushort Red, ushort Green, ushort Blue) HsbToRgb(float h, float s, float v)
    {
        float r, g, b;
        
        if (s == 0)
        {
            r = g = b = v;
        }
        else
        {
            h = (h - MathF.Floor(h)) * 6.0f;
            var i = (int)MathF.Floor(h);
            var f = h - i;
            var p = v * (1.0f - s);
            var q = v * (1.0f - f * s);
            var t = v * (1.0f - (1.0f - f) * s);

            switch (i)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }
        }

        return (
            (ushort)Math.Round(r * ushort.MaxValue),
            (ushort)Math.Round(g * ushort.MaxValue),
            (ushort)Math.Round(b * ushort.MaxValue)
        );
    }
    
    private static (ushort Red, ushort Green, ushort Blue) CmykToRgb(ushort c16, ushort m16, ushort y16, ushort k16)
    {
        var c = 1f - (c16 / 65_535f);
        var m = 1f - (m16 / 65_535f);
        var y = 1f - (y16 / 65_535f);
        var k = 1f - (k16 / 65_535f);
        
        var r = (1f - c) * (1f - k);
        var g = (1f - m) * (1f - k);
        var b = (1f - y) * (1f - k);

        return (
            (ushort)Math.Round(r * ushort.MaxValue),
            (ushort)Math.Round(g * ushort.MaxValue),
            (ushort)Math.Round(b * ushort.MaxValue)
        );
    }
    
    private static (ushort Red, ushort Green, ushort Blue) LabToRgb(ushort l16, short a16, short b16)
    {
        var L = l16 * (100f / 10_000f);
        var a = a16 / 100f;
        var b = b16 / 100f;
        
        var y = (L + 16f) / 116f;
        var x = a / 500f + y;
        var z = y - b / 200f;

        var x3 = x * x * x;
        var y3 = y * y * y;
        var z3 = z * z * z;

        x = (x3 > 0.008856f) ? x3 : (x - 16f / 116f) / 7.787f;
        y = (y3 > 0.008856f) ? y3 : (y - 16f / 116f) / 7.787f;
        z = (z3 > 0.008856f) ? z3 : (z - 16f / 116f) / 7.787f;

        // D65 reference white
        x *= 0.95047f;
        y *= 1.00000f;
        z *= 1.08883f;

        // Step 3. XYZ → linear RGB
        var r =  3.2406f * x - 1.5372f * y - 0.4986f * z;
        var g = -0.9689f * x + 1.8758f * y + 0.0415f * z;
        var bl = 0.0557f * x - 0.2040f * y + 1.0570f * z;

        // Step 4. Gamma correction (sRGB)
        static float Gamma(float c)
        {
            return c <= 0.0031308f ? 12.92f * c : 1.055f * MathF.Pow(c, 1f / 2.4f) - 0.055f;
        }

        r = Gamma(r);
        g = Gamma(g);
        bl = Gamma(bl);

       
        r = Compat.MathCompat.Clamp(r, 0f, 1f);
        g = Compat.MathCompat.Clamp(g, 0f, 1f);
        bl = Compat.MathCompat.Clamp(bl, 0f, 1f);

        return (
            (ushort)Math.Round(r * ushort.MaxValue),
            (ushort)Math.Round(g * ushort.MaxValue),
            (ushort)Math.Round(bl * ushort.MaxValue)
        );
    }
}