using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class VectorStrokeData : TaggedBlock
{
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public VectorStrokeData(byte[] rawData) : base(AdditionalLayerInfoKey.VectorStrokeData, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}