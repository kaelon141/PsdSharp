namespace PsdSharp.TaggedBlocks;

public class TaggedBlock
{
    public AdditionalLayerInfoKey Key { get; }
    public byte[] RawData { get; }

    public TaggedBlock(AdditionalLayerInfoKey key, byte[] rawData)
    {
        Key = key;
        RawData = rawData;
    }
}