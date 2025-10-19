using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class VectorOriginationData : TaggedBlock
{
    public int Version { get; set; }
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public VectorOriginationData(byte[] rawData) : base(AdditionalLayerInfoKey.VectorOriginationData, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        Version = reader.ReadInt32();
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}