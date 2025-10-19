using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class Exposure : TaggedBlock
{
    public short Version { get; }
    public int ExposureValue { get; }
    public int Offset { get; }
    public int Gamma { get; }
    
    public Exposure(byte[] rawData) : base(AdditionalLayerInfoKey.Exposure, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));

        Version = reader.ReadInt16();
        ExposureValue = reader.ReadInt32();
        Offset = reader.ReadInt32();
        Gamma = reader.ReadInt32();
    }
}