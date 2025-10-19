namespace PsdSharp.WPF;

public static class ColorExtensions
{
    public static System.Drawing.Color ToSystemDrawingColor(this PsdColor psdColor)
    {
        var (r16, g16, b16) = psdColor.ToRgb();
        (byte Red, byte Green, byte Blue) rgb8 = ((byte)(r16 >> 8), (byte)(g16 >> 8), (byte)(b16 >> 8));
        return System.Drawing.Color.FromArgb(rgb8.Red, rgb8.Green, rgb8.Blue);
    }
}