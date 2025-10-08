using System.Buffers.Binary;

namespace PsdSharp.Images.DataConversion;

internal static class RgbInterleaver
{
    public static byte[] ToInterleavedBuffer(ImageData imageData, ColorType desiredColorType)
    {
        var channelsArray = imageData.GetChannels().ToArray();
        var channels = new Dictionary<Channel, byte[]>()
        {
            { Channel.Red, channelsArray[0].GetData() },
            { Channel.Green, channelsArray[1].GetData() },
            { Channel.Blue, channelsArray[2].GetData() },
        };
        if (channelsArray.Length > 3)
        {
            channels.Add(Channel.Alpha, channelsArray[3].GetData());
        }

        var sourceBytesPerPixel = imageData.ChannelDepth / 8;
        
        var pixelCount = (int)(imageData.Width * imageData.Height);
        var destBytesPerPixel = (int)Math.Ceiling(desiredColorType.Channels.Sum(x => x.PixelCount) / 8d);
        var interleavedBuffer = new byte[pixelCount * destBytesPerPixel];
        var destinationChannels = desiredColorType.Channels;

        var chunkSize = 50_000;
        Parallel.For(0, (pixelCount + chunkSize - 1) / chunkSize, chunk =>
        {
            var start = chunk * chunkSize;
            var end = Math.Min(start + chunkSize, pixelCount);

            for (var i = start; i < end; i++)
            {
                var srcOffset = i * sourceBytesPerPixel;
                var destOffset = i * destBytesPerPixel;

                for (var j = 0; j < destinationChannels.Length; j++)
                {
                    if (!channels.ContainsKey(destinationChannels[j].Channel))
                    {
                        interleavedBuffer[destOffset + j] = 255;
                        continue;
                    }

                    var channel = channels[destinationChannels[j].Channel];

                    interleavedBuffer[destOffset + j] = sourceBytesPerPixel switch
                    {
                        1 => channel[srcOffset],
                        2 => (byte)(BinaryPrimitives.ReadUInt16BigEndian(channel.AsSpan(srcOffset, 2)) >> 8),
                        _ => (byte)(Math.Clamp(BinaryPrimitives.ReadSingleBigEndian(channel.AsSpan(srcOffset, 4)), 0,
                            1) * 255)
                    };
                }
            }
        });
        
        return interleavedBuffer;
    }
}