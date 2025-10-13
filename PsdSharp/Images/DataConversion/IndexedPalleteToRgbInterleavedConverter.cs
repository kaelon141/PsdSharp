namespace PsdSharp.Images.DataConversion;

internal static class IndexedPalleteToRgbInterleavedConverter
{
    public static byte[] Convert(ImageData imageData, ColorType destinationColorType)
    {
        var red = imageData.ColorModeData.AsSpan(0, 256).ToArray();
        var green = imageData.ColorModeData.AsSpan(256, 256).ToArray();
        var blue = imageData.ColorModeData.AsSpan(512, 256).ToArray();
        
        var sourceData = imageData.GetChannels().First().GetData();
        
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
                var colorIndex = sourceData[i];
                var destOffset = i * destBytesPerPixel;

                for (var j = 0; j < destinationChannels.Length; j++)
                {
                    interleavedBuffer[destOffset + j] = destinationChannels[j].Channel switch
                    {
                        Channel.Red => red[colorIndex],
                        Channel.Green => green[colorIndex],
                        Channel.Blue => blue[colorIndex],
                        _ => 255,
                    };
                }
            }
        });
        
        return interleavedBuffer;
    }
}