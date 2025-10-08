using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PsdSharp.Parsing;
using PsdSharp.SkiaSharp;
using SkiaSharp;

namespace PsdSharp.Avalonia;

public class PsdView : global::Avalonia.Controls.Control
{
    public static readonly DirectProperty<PsdView, Stream?> PsdFileProperty =
        AvaloniaProperty.RegisterDirect<PsdView, Stream?>(
            nameof(PsdFile),
            o => o.PsdFile,
            (o, v) => o.PsdFile = v);

    private Stream? _psdFile;
    private SKBitmap? _skBitmap;

    public Stream? PsdFile
    {
        get => _psdFile;
        set
        {
            SetAndRaise(PsdFileProperty, ref _psdFile, value);
            if (value is not null)
            {
                if (value.CanSeek)
                    value.Seek(0, SeekOrigin.Begin);
                
                var psdFile = global::PsdSharp.PsdFile.Open(value);
                _skBitmap = psdFile.ImageData!.ToSkBitmap();
            }
            else
            {
                _skBitmap = null;
            }

            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_skBitmap is null)
            return;

        // Copy SKBitmap pixels into Avalonia's drawing pipeline
        using var wb = new WriteableBitmap(
            new PixelSize(_skBitmap.Width, _skBitmap.Height),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Premul);

        using (var fb = wb.Lock())
        {
            var info = new SKImageInfo(fb.Size.Width, fb.Size.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using (var surface = SKSurface.Create(info, fb.Address, fb.RowBytes))
            {
                surface.Canvas.DrawBitmap(_skBitmap, 0, 0);
            }
        }

        var destRect = new Rect(Bounds.Size);
        context.DrawImage(wb, new Rect(wb.Size), destRect);
    }
}