using System.Reflection;
using PsdSharp.SkiaSharp;
using Silk.NET.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;

namespace PsdSharp.Samples.SkiaSharp;

public class Program
{
    private static IWindow _window;
    private static GL _gl;
    private static SKSurface _surface;
    private static SKBitmap _psdFileBitmap;
    
    public static void Main(string[] args)
    {
        var psdFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("PsdSharp.Samples.SkiaSharp.Assets.test.psd");
        var psd = PsdFile.Open(psdFile!);
        _psdFileBitmap = psd.ImageData!.ToSkBitmap();
        
        var windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1024, 1024),
            Title = "PsdSharp.Samples.SkiaSharp"
        };

        _window = Window.Create(windowOptions);
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Run();
    }

    private static void OnLoad()
    {
        _window.MakeCurrent();
        _gl = _window.CreateOpenGL();
        
        var glInterface = GRGlInterface.Create(name => _gl.Context.TryGetProcAddress(name, out var procAddr) ? procAddr : IntPtr.Zero);
        var grContext = GRContext.CreateGl(glInterface);

        _gl.GetInteger(GLEnum.Samples, out var samples);
        _gl.GetInteger((GLEnum)0x0D57, out var stencilBits);
        
        var fbInfo = new GRGlFramebufferInfo(0, (uint)SizedInternalFormat.Rgba8);
        
        var renderTarget = new GRBackendRenderTarget(_window.Size.X, _window.Size.Y, samples, stencilBits,
            fbInfo);
        _surface = SKSurface.Create(grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
    }
    
    private static void OnRender(double delta)
    {
        _surface.Canvas.Clear(new SKColor(39, 39, 39));
        _surface.Canvas.DrawBitmap(_psdFileBitmap, new SKRect(0, 0, _window.Size.X, _window.Size.Y));
        _surface.Canvas.Flush();
    }
}

