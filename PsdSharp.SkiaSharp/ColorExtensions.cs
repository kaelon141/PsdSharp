using SkiaSharp;

namespace PsdSharp.SkiaSharp;

public static class ColorExtensions
{
    public static SKColor ToSkColor(this PsdColor psdColor)
    {
        var (r16, g16, b16) = psdColor.ToRgb();
        
        var skColor =
            0xFF000000u |
            ((uint)(r16 >> 8) << 16) |
            ((uint)(g16 >> 8) << 8) |
            (uint)(b16 >> 8);

        return new SKColor(skColor);
    }
}