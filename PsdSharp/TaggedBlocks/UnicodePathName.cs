using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class UnicodePathName : TaggedBlock
{
    public int DescriptorVersion { get; }
    public Descriptor Descriptor { get; }
    
    public UnicodePathName(byte[] rawData) : base(AdditionalLayerInfoKey.UnicodePathName, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}