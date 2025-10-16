using System.Reflection;
using Avalonia.Controls;

namespace PsdSharp.Samples.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var psdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PsdSharp.Samples.Avalonia.Assets.test.psd");
        PsdView.PsdFile = psdStream;
    }
}