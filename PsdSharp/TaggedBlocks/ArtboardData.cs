using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class ArtboardData : TaggedBlock
{
    public int DescriptorVersion { get; }
    public Descriptor Descriptor { get; }
    
    public ArtboardData(AdditionalLayerInfoKey key, byte[] rawData) : base(key, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}