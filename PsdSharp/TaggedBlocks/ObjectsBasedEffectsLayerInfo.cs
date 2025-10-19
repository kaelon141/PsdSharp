using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class ObjectsBasedEffectsLayerInfo : TaggedBlock
{
    public uint Version { get; set; }
    public uint DescriptorVersion { get; set; }
    
    public Descriptor Descriptor { get; set; }
    
    public ObjectsBasedEffectsLayerInfo(byte[] rawData) : base(AdditionalLayerInfoKey.ObjectsBasedEffectsLayerInfo, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        Version = reader.ReadUInt32();
        DescriptorVersion = reader.ReadUInt32();
        
        Descriptor = new Descriptor(reader);
    }
}