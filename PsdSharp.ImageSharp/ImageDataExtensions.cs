using PsdSharp.Images;
using PsdSharp.Images.DataConversion;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PsdSharp.ImageSharp;

public static class ImageDataExtensions
{
   public static Image<Rgba32> ToImageSharpImage(this ImageData imageData)
   {
      var interleavedBuffer = PixelDataConverter.GetInterleavedBuffer(imageData, ColorType.Rgba8888);
      return Image.LoadPixelData<Rgba32>(interleavedBuffer, (int)imageData.Width, (int)imageData.Height);
   }
}