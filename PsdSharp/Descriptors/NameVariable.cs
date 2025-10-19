using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class NameVariable : Variable
{
    public readonly string Name;
    public override object Value => Name;
    
    internal NameVariable(string key, BigEndianReader reader) : base(key)
    {
        Name = reader.ReadUnicodeString().String!;
    }
}