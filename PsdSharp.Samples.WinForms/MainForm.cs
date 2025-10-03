using System.Reflection;
using PsdSharp.Win32;

namespace PsdSharp.Samples.WinForms;

public class MainForm : Form
{
    private readonly Bitmap _bitmap;

    public MainForm()
    {
        var psdFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("PsdSharp.Samples.WinForms.Assets.test.psd");
        _bitmap = PsdFile.Open(psdFile!).ImageData!.ToBitmap();
        
        Text = "PsdSharp.Samples.WinForms";
        Width = 1024;
        Height = 1024;
        BackColor = Color.FromArgb(255, 39, 39, 39);
       
        
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_bitmap != null)
        {
            e.Graphics.DrawImage(_bitmap, ClientRectangle);
        }
    }
}