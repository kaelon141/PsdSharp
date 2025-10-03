using PsdSharp.Images;
using SkiaSharp;

namespace PsdSharp.SkiaSharp;

public static class ImageDataExtensions
{
    public static SKBitmap ToSkBitmap(this ImageData imageData)
    {
        var channels = imageData.GetChannels().ToArray();
        var red = channels[0].GetData();
        var green = channels[1].GetData();
        var blue = channels[2].GetData();
        var alpha = channels.Length > 3 ? channels[3].GetData() : null;
        
        var pixelCount = imageData.Width * imageData.Height;
        var interleavedBuffer = new byte[pixelCount * 4];

        for (int i = 0, j = 0; i < pixelCount; i++, j += 4)
        {
            interleavedBuffer[j] = red[i];
            interleavedBuffer[j + 1] = green[i];
            interleavedBuffer[j + 2] = blue[i];
            interleavedBuffer[j + 3] = alpha != null ? alpha[i] : (byte)0;
        }
        
        var bitmap = new SKBitmap(new SKImageInfo((int)imageData.Width, (int)imageData.Height, alpha is not null ? SKColorType.Rgba8888 : SKColorType.Rgb888x, SKAlphaType.Unpremul));
        System.Runtime.InteropServices.Marshal.Copy(interleavedBuffer, 0, bitmap.GetPixels(), interleavedBuffer.Length);
        
        return bitmap;
    }
}