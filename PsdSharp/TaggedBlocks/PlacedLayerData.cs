using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class PlacedLayerData : TaggedBlock
{
    public int Version { get; }
    public int DescriptorVersion { get; }
    public Descriptor Descriptor { get; }
    
    public PlacedLayerData(byte[] rawData) : base(AdditionalLayerInfoKey.SmartObjectLayerData1, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        reader.Skip(4);
        Version = reader.ReadInt32();
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}