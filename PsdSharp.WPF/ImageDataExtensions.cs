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
}