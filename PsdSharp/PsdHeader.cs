namespace PsdSharp;

public class PsdHeader
{
    public PsdFileType PsdFileType { get; set; }
    public ushort NumberOfChannels { get; set; }
    public uint HeightInPixels { get; set; }
    public uint WidthInPixels { get; set; }
    public byte ChannelDepth { get; set; }
    public ColorMode ColorMode { get; set; }
    public byte[] ColorModeData { get; internal set; } = [];
}