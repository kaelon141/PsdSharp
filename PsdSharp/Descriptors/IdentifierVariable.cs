using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class IdentifierVariable : Variable
{
    public readonly string Identifier;
    public override object Value => Identifier;
    
    internal IdentifierVariable(string key, BigEndianReader reader) : base(key)
    {
        Identifier = reader.ReadFixedLengthString(4);
    }
}