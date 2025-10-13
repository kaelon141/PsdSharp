using System.Buffers.Binary;

namespace PsdSharp.Images.DataConversion;

internal static class LabPlanarToRgbInterleavedConverter
{
    public static byte[] Convert(ImageData imageData, ColorType destinationColorType)
    {
        var pixelCount = (int)(imageData.Width * imageData.Height);
        var bytesPerSample = imageData.ChannelDepth / 8;
        var destBytesPerPixel = (int)Math.Ceiling(destinationColorType.Channels.Sum(x => x.PixelCount) / 8d);
        var interleavedBuffer = new byte[pixelCount * destBytesPerPixel];
        
        var channels = imageData.GetChannels().ToArray();
        var lSrc = To8Bit(channels[0].GetData(), bytesPerSample, pixelCount);
        var aSrc = To8Bit(channels[1].GetData(), bytesPerSample, pixelCount);
        var bSrc = To8Bit(channels[2].GetData(), bytesPerSample, pixelCount);
        
        const int chunkSize = 50_000;
         Parallel.For(0L, (pixelCount + chunkSize - 1) / chunkSize, chunk =>
        {
            var start = chunk * chunkSize;
            var end = Math.Min(start + chunkSize, pixelCount);

            for (long i = start, dst = start * 4; i < end; i++, dst += 4)
            {
                // --- 1. Decode Lab bytes to floats ---
                var l = (lSrc[i] / 255f) * 100f;
                var a = aSrc[i] - 128f;
                var b = bSrc[i] - 128f;

                // --- 2. Lab → XYZ (D65) ---
                var y = (l + 16f) / 116f;
                var x = a / 500f + y;
                var z = y - b / 200f;

                x = x > 0.206893f ? x * x * x : (x - 16f / 116f) / 7.787f;
                y = y > 0.206893f ? y * y * y : (y - 16f / 116f) / 7.787f;
                z = z > 0.206893f ? z * z * z : (z - 16f / 116f) / 7.787f;

                x *= 95.047f;
                y *= 100.000f;
                z *= 108.883f;

                // --- 3. XYZ → linear RGB ---
                var r =  3.2406f * x / 100f - 1.5372f * y / 100f - 0.4986f * z / 100f;
                var g = -0.9689f * x / 100f + 1.8758f * y / 100f + 0.0415f * z / 100f;
                var bl = 0.0557f * x / 100f - 0.2040f * y / 100f + 1.0570f * z / 100f;

                // --- 4. Gamma correction (sRGB) ---
                r = r <= 0f ? 0f : r >= 1f ? 1f :
                    (r > 0.0031308f ? (1.055f * MathF.Pow(r, 1f / 2.4f) - 0.055f) : (12.92f * r));
                g = g <= 0f ? 0f : g >= 1f ? 1f :
                    (g > 0.0031308f ? (1.055f * MathF.Pow(g, 1f / 2.4f) - 0.055f) : (12.92f * g));
                bl = bl <= 0f ? 0f : bl >= 1f ? 1f :
                    (bl > 0.0031308f ? (1.055f * MathF.Pow(bl, 1f / 2.4f) - 0.055f) : (12.92f * bl));

                // --- 5. Store RGBA ---
                interleavedBuffer[dst + 0] = (byte)(r * 255f + 0.5f);
                interleavedBuffer[dst + 1] = (byte)(g * 255f + 0.5f);
                interleavedBuffer[dst + 2] = (byte)(bl * 255f + 0.5f);
                interleavedBuffer[dst + 3] = 255; // opaque
            }
        });
         
         return interleavedBuffer;
    }
    
    private static byte[] To8Bit(byte[] src, int bytesPerSample, int pixelCount)
    {
        var dst = new byte[pixelCount];

        switch (bytesPerSample)
        {
            case 1:
                Buffer.BlockCopy(src, 0, dst, 0, pixelCount);
                break;

            case 2:
                for (int i = 0, s = 0; i < pixelCount; i++, s += 2)
                {
                    var v16 = BinaryPrimitives.ReadUInt16BigEndian(src.AsSpan(s, 2));
                    dst[i] = (byte)(v16 / 257); // 0..65535 -> 0..255
                }
                break;

            case 4:
                for (int i = 0, s = 0; i < pixelCount; i++, s += 4)
                {
                    #if NET6_0_OR_GREATER
                    var f = BinaryPrimitives.ReadSingleBigEndian(src.AsSpan(s, 4));
                    #else
                    var f = Compat.BinaryPrimitivesCompat.ReadSingleBigEndian(src.AsSpan(s, 4));
                    #endif
                    if (f < 0f) f = 0f;
                    else if (f > 1f) f = 1f;
                    dst[i] = (byte)(f * 255f + 0.5f);
                }
                break;

            default:
                throw new NotSupportedException($"Unsupported channel depth: {bytesPerSample * 8} bits");
        }

        return dst;
    }
}