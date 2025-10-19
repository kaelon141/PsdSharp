using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class AnimationEffects : TaggedBlock
{
    public int DescriptorVersion { get; }
    public Descriptor Descriptor { get; }
    
    public AnimationEffects(byte[] rawData) : base(AdditionalLayerInfoKey.AnimationEffects, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}