namespace PsdSharp.TaggedBlocks;

public class TextEngineData : TaggedBlock
{
    public byte[] DataForTextEngine { get; set; }
    
    public TextEngineData(byte[] rawData) : base(AdditionalLayerInfoKey.TextEngineData, rawData)
    {
        DataForTextEngine = rawData.AsSpan(4).ToArray();
    }
}