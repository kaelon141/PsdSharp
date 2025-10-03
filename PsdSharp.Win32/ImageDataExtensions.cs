using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PsdSharp.Images;

namespace PsdSharp.Win32;

public static class ImageDataExtensions
{
    public static Bitmap ToBitmap(this ImageData imageData)
    {
        var data = GetInterleavedBuffer(imageData);
        
        var bitmap = new Bitmap((int)imageData.Width, (int)imageData.Height, data.HasAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb);
        var rect = new System.Drawing.Rectangle(0, 0, (int)imageData.Width, (int)imageData.Height);
        var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
        
        Marshal.Copy(data.Buffer, 0, bmpData.Scan0, data.Buffer.Length);
        bitmap.UnlockBits(bmpData);
        return bitmap;
    }
    
    private static (byte[] Buffer, bool HasAlpha) GetInterleavedBuffer(ImageData imageData)
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
            interleavedBuffer[j] = blue[i];
            interleavedBuffer[j + 1] = green[i];
            interleavedBuffer[j + 2] = red[i];
            interleavedBuffer[j + 3] = alpha != null ? alpha[i] : (byte)0;
        }

        return (interleavedBuffer, alpha != null);
    }
}