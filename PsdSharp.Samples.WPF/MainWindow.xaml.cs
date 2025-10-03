using System.Reflection;
using System.Windows;
using PsdSharp.WPF;

namespace PsdSharp.Samples.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var psdFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("PsdSharp.Samples.WPF.Assets.test.psd");
        Image.Source = PsdFile.Open(psdFile!).ImageData!.ToBitmapSource();
    }
}