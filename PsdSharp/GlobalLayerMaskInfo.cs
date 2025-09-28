namespace PsdSharp;

public class GlobalLayerMaskInfo
{
    /// <summary>
    /// Undocumented.
    /// </summary>
    /// <returns></returns>
    public ushort OverlayColorSpace { get; set; }
    
    public ushort ColorComponent1 { get; set; }
    public ushort ColorComponent2{ get; set; }
    public ushort ColorComponent3 { get; set; }
    public ushort ColorComponent4 { get; set; }

    private ushort _opacity;
    public ushort Opacity
    {
        get => _opacity;
        set
        {
            if (value > 100) throw new ArgumentOutOfRangeException("Opacity must be between 0 and 100");
            _opacity = value;
        }
    }
    
    public KindEnum Kind { get; set; }

    public enum KindEnum : byte
    {
        ColorSelected = 0,
        ColorProtected = 1,
        UseValueStoredPerlayer = 128
    }
}