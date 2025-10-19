using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class Vibrance : TaggedBlock
{
    public uint DescriptorVersion { get; set; }
    
    public Descriptor Descriptor { get; set; }
    
    public Vibrance(byte[] rawData) : base(AdditionalLayerInfoKey.ObjectsBasedEffectsLayerInfo, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadUInt32();
        
        Descriptor = new Descriptor(reader);
    }
}