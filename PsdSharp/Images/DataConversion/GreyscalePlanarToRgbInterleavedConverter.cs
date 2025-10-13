using System.Buffers.Binary;

namespace PsdSharp.Images.DataConversion;

internal static class GreyscalePlanarToRgbInterleavedConverter
{
    public static byte[] Convert(ImageData imageData, ColorType destinationColorType)
    {
        var sourceData = imageData.GetChannels().First().GetData();
        var sourceBytesPerPixel = imageData.ChannelDepth / 8;
        
        var pixelCount = (int)(imageData.Width * imageData.Height);
        var destBytesPerPixel = (int)Math.Ceiling(destinationColorType.Channels.Sum(x => x.PixelCount) / 8d);
        var interleavedBuffer = new byte[pixelCount * destBytesPerPixel];
        var destinationChannels = destinationColorType.Channels;
        
        var chunkSize = 50_000;
        Parallel.For(0, (pixelCount + chunkSize - 1) / chunkSize, chunk =>
        {
            var start = chunk * chunkSize;
            var end = Math.Min(start + chunkSize, pixelCount);

            for (var i = start; i < end; i++)
            {
                var srcOffset = i * sourceBytesPerPixel;
                var destOffset = i * destBytesPerPixel;
                
                var greyscale = sourceBytesPerPixel switch
                {
                    1 => sourceData[srcOffset],
                    2 => (byte)(BinaryPrimitives.ReadUInt16BigEndian(sourceData.AsSpan(srcOffset, 2)) >> 8),
                    #if NET6_0_OR_GREATER
                    _ => (byte)(Math.Clamp(BinaryPrimitives.ReadSingleBigEndian(sourceData.AsSpan(srcOffset, 4)), 0, 1) * 255)
                    #else
                    _ => (byte)(Compat.MathCompat.Clamp(Compat.BinaryPrimitivesCompat.ReadSingleBigEndian(sourceData.AsSpan(srcOffset, 4)), 0, 1) * 255)
                    #endif
                };

                for (var j = 0; j < destinationChannels.Length; j++)
                {
                    interleavedBuffer[destOffset + j] = destinationChannels[j].Channel == Channel.Alpha ? (byte)255 : greyscale;
                }
            }
        });
        
        return interleavedBuffer;
    }
}