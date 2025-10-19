using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class ColorLookup : TaggedBlock
{
    public short Version { get; set; }
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public ColorLookup(byte[] rawData) : base(AdditionalLayerInfoKey.ColorLookup, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        Version = reader.ReadInt16();
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}