namespace PsdSharp.TaggedBlocks;

public class UsingAlignedRendering : TaggedBlock
{
    public bool IsUsingAlignedRendering { get; }
    
    public UsingAlignedRendering(byte[] rawData) : base(AdditionalLayerInfoKey.UsingAlignedRendering, rawData)
    {
        IsUsingAlignedRendering = !(rawData[0] == 0 && rawData[1] == 0 && rawData[2] == 0 && rawData[3] == 0);
    }
}