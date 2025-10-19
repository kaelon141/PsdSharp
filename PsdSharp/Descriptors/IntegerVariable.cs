using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class IntegerVariable : Variable
{
    public readonly int ValueInt;
    public override object Value => ValueInt;
    
    internal IntegerVariable(string key, BigEndianReader reader) : base(key)
    {
        ValueInt = reader.ReadInt32();
    }
}