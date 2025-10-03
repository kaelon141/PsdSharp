using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;

namespace PsdSharp.Samples.Avalonia;

public class SkiaControl : Control
{
    public static readonly DirectProperty<SkiaControl, SKBitmap?> BitmapProperty =
        AvaloniaProperty.RegisterDirect<SkiaControl, SKBitmap?>(
            nameof(Bitmap),
            o => o.Bitmap,
            (o, v) => o.Bitmap = v);

    private SKBitmap? _bitmap;

    public SKBitmap? Bitmap
    {
        get => _bitmap;
        set
        {
            SetAndRaise(BitmapProperty, ref _bitmap, value);
            InvalidateVisual(); // force redraw when bitmap changes
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bitmap is null)
            return;

        // Copy SKBitmap pixels into Avalonia's drawing pipeline
        using var wb = new WriteableBitmap(
            new PixelSize(Bitmap.Width, Bitmap.Height),
            new Vector(96, 96),
            Bitmap.ColorType.ToPixelFormat(),
            AlphaFormat.Premul);

        using (var fb = wb.Lock())
        {
            var info = new SKImageInfo(fb.Size.Width, fb.Size.Height, Bitmap.ColorType, SKAlphaType.Premul);
            using (var surface = SKSurface.Create(info, fb.Address, fb.RowBytes))
            {
                surface.Canvas.DrawBitmap(Bitmap, 0, 0);
            }
        }

        var destRect = new Rect(Bounds.Size);
        context.DrawImage(wb, new Rect(wb.Size), destRect);
    }
}