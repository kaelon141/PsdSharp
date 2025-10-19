using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class StringVariable : Variable
{
    public readonly string ValueString;
    public override object Value => ValueString;
    
    internal StringVariable(string key, BigEndianReader reader) : base(key)
    {
        ValueString = reader.ReadUnicodeString().String!;
    }
}