namespace PsdSharp;

public class PsdHeader
{
    public required PsdFileType PsdFileType { get; init; }
    public required ushort NumberOfChannels { get; init; }
    public required uint HeightInPixels { get; init; }
    public required uint WidthInPixels { get; init; }
    public required byte ChannelDepth { get; init; }
    public required ColorMode ColorMode { get; init; }
    public byte[] ColorModeData { get; internal set; }
}