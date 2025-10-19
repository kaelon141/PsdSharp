using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class BlackWhite : TaggedBlock
{
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public BlackWhite(byte[] rawData) : base(AdditionalLayerInfoKey.BlackWhite, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}