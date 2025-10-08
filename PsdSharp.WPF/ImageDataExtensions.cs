using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PsdSharp.Images;
using PsdSharp.Images.DataConversion;

namespace PsdSharp.WPF;

public static class ImageDataExtensions
{
    public static BitmapSource ToBitmapSource(this ImageData imageData)
    {
        var interleavedBuffer = PixelDataConverter.GetInterleavedBuffer(imageData, ColorType.Bgra8888);
        
        var wb = new WriteableBitmap((int)imageData.Width, (int)imageData.Height, 96, 96, PixelFormats.Bgra32, null);
        wb.WritePixels(new Int32Rect(0, 0, (int)imageData.Width, (int)imageData.Height), interleavedBuffer, 4 * (int)imageData.Width, 0);
        return wb;
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