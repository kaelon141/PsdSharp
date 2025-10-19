namespace PsdSharp.Avalonia;

public static class ColorExtensions
{
    public static global::Avalonia.Media.Color ToAvaloniaColor(this PsdColor psdColor)
    {
        var (r16, g16, b16) = psdColor.ToRgb();

        return global::Avalonia.Media.Color.FromRgb((byte)(r16 >> 8), (byte)(g16 >> 8), (byte)(b16 >> 8));
    }
}