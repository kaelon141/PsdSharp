using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class UserMask : TaggedBlock
{
    public byte[] ColorSpace { get; set; }
    public short Opacity { get; set; }
    public byte Flag { get; set; }
    
    public UserMask(byte[] rawData) : base(AdditionalLayerInfoKey.UserMask, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        ColorSpace = new byte[10];
        reader.ReadIntoBuffer(ColorSpace);
        
        Opacity = reader.ReadInt16();
        Flag = reader.ReadByte();
    }
}