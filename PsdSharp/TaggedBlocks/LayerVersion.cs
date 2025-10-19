using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class LayerVersion : TaggedBlock
{
    public int MinimumPhotoshopVersionRequired;
    
    public LayerVersion(byte[] rawData) : base(AdditionalLayerInfoKey.LayerVersion, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        MinimumPhotoshopVersionRequired = reader.ReadInt32();
    }
}