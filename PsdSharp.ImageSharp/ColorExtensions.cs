using SixLabors.ImageSharp.PixelFormats;

namespace PsdSharp.ImageSharp;

public static class ColorExtensions
{
    public static SixLabors.ImageSharp.Color ToImageSharpColor(this PsdColor psdColor)
    {
        var rgb = psdColor.ToRgb();
        return new SixLabors.ImageSharp.Color(new Rgb48(rgb.Red, rgb.Green, rgb.Blue));
    }
}