using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class IndexVariable : Variable
{
    public readonly int Index;
    public override object Value => Index;
    
    internal IndexVariable(string key, BigEndianReader reader) : base(key)
    {
        Index = reader.ReadInt32();
    }
}