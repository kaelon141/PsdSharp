using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class BrightnessAndContract : TaggedBlock
{
    public short Brightness { get; set; }
    public short Contrast { get; set; }
    public short MeanValue { get; set; }
    public bool LabColorOnly { get; set; }
    
    public BrightnessAndContract(byte[] rawData) : base(AdditionalLayerInfoKey.BrightnessAndContract, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        Brightness = reader.ReadInt16();
        Contrast = reader.ReadInt16();
        MeanValue = reader.ReadInt16();
        LabColorOnly = reader.ReadByte() != 0;
    }
}