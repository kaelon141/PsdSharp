using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PsdSharp.Images;
using PsdSharp.Images.DataConversion;

namespace PsdSharp.Win32;

public static class ImageDataExtensions
{
    public static Bitmap ToBitmap(this ImageData imageData)
    {
        var interleavedBuffer = PixelDataConverter.GetInterleavedBuffer(imageData, ColorType.Bgra8888);
        var bitmap = new Bitmap((int)imageData.Width, (int)imageData.Height, PixelFormat.Format32bppArgb);
        var rect = new System.Drawing.Rectangle(0, 0, (int)imageData.Width, (int)imageData.Height);
        var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
        
        Marshal.Copy(interleavedBuffer, 0, bmpData.Scan0, interleavedBuffer.Length);
        bitmap.UnlockBits(bmpData);
        return bitmap;
    }
}