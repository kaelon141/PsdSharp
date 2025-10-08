namespace PsdSharp.Images.DataConversion;

public static class PixelDataConverter
{
    public static byte[] GetInterleavedBuffer(ImageData imageData, ColorType colorType)
    {
        if (imageData.ColorMode is ColorMode.Rgb or ColorMode.Cmyk)
        {
            return RgbInterleaver.ToInterleavedBuffer(imageData, colorType);
        }

        if (imageData.ColorMode == ColorMode.Bitmap)
        {
            return BitmapPlanarToRgbInterleavedConverter.Convert(imageData, colorType);
        }

        if (imageData.ColorMode is ColorMode.Grayscale or ColorMode.Duotone or ColorMode.Multichannel)
        {
            return GreyscalePlanarToRgbInterleavedConverter.Convert(imageData, colorType);
        }

        if (imageData.ColorMode == ColorMode.Indexed)
        {
            return IndexedPalleteToRgbInterleavedConverter.Convert(imageData, colorType);
        }

        if (imageData.ColorMode == ColorMode.Lab)
        {
            return LabPlanarToRgbInterleavedConverter.Convert(imageData, colorType);
        }

        throw new NotSupportedException($"Color mode {imageData.ColorMode} is not supported.");
    }
}