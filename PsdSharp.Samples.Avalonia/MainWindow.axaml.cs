using System.IO;
using System.Reflection;
using Avalonia.Controls;
using PsdSharp.Parsing;
using PsdSharp.SkiaSharp;

namespace PsdSharp.Samples.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var psdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PsdSharp.Samples.Avalonia.Assets.test.psd");
        var psdFile = PsdFile.Open(psdStream!);
        SkiaView.Bitmap = psdFile.ImageData!.ToSkBitmap();
        
        PsdView.PsdFile = psdStream;
    }
}