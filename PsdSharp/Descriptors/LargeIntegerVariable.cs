using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class LargeIntegerVariable : Variable
{
    public readonly long ValueLong;
    public override object Value => ValueLong;
    
    internal LargeIntegerVariable(string key, BigEndianReader reader) : base(key)
    {
        ValueLong = reader.ReadInt64();
    }
}