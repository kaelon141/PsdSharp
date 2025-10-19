using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class DoubleVariable : Variable
{
    public readonly double ValueDouble;
    public override object Value => ValueDouble;
    
    internal DoubleVariable(string key, BigEndianReader reader) : base(key)
    {
        ValueDouble = reader.ReadDouble();
    }
}