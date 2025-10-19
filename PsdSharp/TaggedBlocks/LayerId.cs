using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class LayerId : TaggedBlock
{
    public int Id { get; set; }
    
    public LayerId(byte[] rawData) : base(AdditionalLayerInfoKey.LayerId, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        Id = reader.ReadInt32();
    }
}