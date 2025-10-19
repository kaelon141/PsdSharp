using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class VectorStrokeContentData : TaggedBlock
{
    public new string Key { get; set; }
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public VectorStrokeContentData(byte[] rawData) : base(AdditionalLayerInfoKey.VectorStrokeContentData, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        Key = reader.ReadFixedLengthString(4);
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}