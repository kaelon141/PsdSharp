using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class PixelSourceDataCc : TaggedBlock
{
    public int DescriptorVersion { get; set; }
    public Descriptor Descriptor { get; set; }
    
    public PixelSourceDataCc(byte[] rawData) : base(AdditionalLayerInfoKey.PixelSourceDataCc, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        DescriptorVersion = reader.ReadInt32();
        Descriptor = new Descriptor(reader);
    }
}