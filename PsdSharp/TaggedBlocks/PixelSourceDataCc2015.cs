namespace PsdSharp.TaggedBlocks;

public class PixelSourceDataCc2015(byte[] rawData) : TaggedBlock(AdditionalLayerInfoKey.PixelSourceDataCc2015, rawData)
{
    public new byte[] RawData { get; } = rawData.AsSpan(8).ToArray();
}