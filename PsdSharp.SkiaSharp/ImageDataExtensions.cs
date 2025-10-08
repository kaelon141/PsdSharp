using PsdSharp.Images;
using PsdSharp.Images.DataConversion;
using SkiaSharp;

namespace PsdSharp.SkiaSharp;

public static class ImageDataExtensions
{
    public static SKBitmap ToSkBitmap(this ImageData imageData)
    {
        var interleavedBuffer = PixelDataConverter.GetInterleavedBuffer(imageData, ColorType.Rgba8888);
        
        var bitmap = new SKBitmap(new SKImageInfo((int)imageData.Width, (int)imageData.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul));
        System.Runtime.InteropServices.Marshal.Copy(interleavedBuffer, 0, bitmap.GetPixels(), interleavedBuffer.Length);
        
        return bitmap;
    }
}