using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class SmartObjectLayerData : TaggedBlock
{
    public int Version { get; }
    public Descriptor Descriptor { get; }
    
    public SmartObjectLayerData(byte[] rawData) : base(AdditionalLayerInfoKey.SmartObjectLayerData2, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        reader.Skip(4);
        Version = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}