using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class EnumVariable : Variable
{
    public readonly string TypeId;
    public readonly string Enum;
    public override object Value => (TypeId, Enum);
    
    internal EnumVariable(string key, BigEndianReader reader) : base(key)
    {
        var typeIdLength = reader.ReadInt32();
        TypeId = reader.ReadFixedLengthString(typeIdLength > 0 ? typeIdLength : 4);
        
        var enumLength = reader.ReadInt32();
        Enum = reader.ReadFixedLengthString(enumLength > 0 ? enumLength : 4);
    }
}