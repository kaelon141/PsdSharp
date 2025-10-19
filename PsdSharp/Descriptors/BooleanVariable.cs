using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class BooleanVariable : Variable
{
    public readonly bool ValueBool;
    public override object Value => ValueBool;
    
    internal BooleanVariable(string key, BigEndianReader reader) : base(key)
    {
        ValueBool = reader.ReadByte() != 0;
    }
}