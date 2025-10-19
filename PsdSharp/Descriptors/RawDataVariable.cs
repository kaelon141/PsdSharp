using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class RawDataVariable : Variable
{
    public readonly byte[] RawData;
    public override object Value => RawData;

    internal RawDataVariable(string key, BigEndianReader reader) : base(key)
    {
        var length = reader.ReadUInt32();
        
        RawData = new byte[length];
        reader.ReadIntoBuffer(RawData);
    }
}