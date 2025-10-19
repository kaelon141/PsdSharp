using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class ContentGeneratorExtraData : TaggedBlock
{
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public ContentGeneratorExtraData(byte[] rawData) : base(AdditionalLayerInfoKey.ContentGeneratorExtraData, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}