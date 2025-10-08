namespace PsdSharp.Images.DataConversion;

internal static class BitmapPlanarToRgbInterleavedConverter
{
    public static byte[] Convert(ImageData imageData, ColorType destinationColorType)
    {
        var sourceData = imageData.GetChannels().First().GetData();
        
        var rowBytes = (imageData.Width + 7) / 8;
        var interleavedBuffer = new byte[imageData.Width * imageData.Height * 4];
        var destinationChannels = destinationColorType.Channels;
        
        for (var y = 0; y < imageData.Height; y++)
        {
            var srcOffset = y * rowBytes;

            for (var x = 0; x < imageData.Width; x++)
            {
                var byteIndex = srcOffset + (x >> 3);
                var bitIndex = 7 - (x & 7); 
                var isWhite = ((sourceData[byteIndex] >> bitIndex) & 1) == 0;
                
                var dstIndex = (y * imageData.Width + x) * 4;

                var val = (byte)(isWhite ? 255 : 0);
                interleavedBuffer[dstIndex] = destinationChannels[0].Channel == Channel.Alpha ? (byte)255 : val;
                interleavedBuffer[dstIndex + 1] = destinationChannels[1].Channel == Channel.Alpha ? (byte)255 : val;;
                interleavedBuffer[dstIndex + 2] = destinationChannels[2].Channel == Channel.Alpha ? (byte)255 : val;;
                interleavedBuffer[dstIndex + 3] = destinationChannels[3].Channel == Channel.Alpha ? (byte)255 : val;;
            }
        }

        return interleavedBuffer;
    }
}