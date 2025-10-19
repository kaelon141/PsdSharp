using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class EnumReferenceVariable : Variable
{
    public readonly string Name;
    public readonly string ClassId;
    public readonly string TypeId;
    public readonly string Enum;
    public override object Value => (Name, ClassId, TypeId, Enum);
    
    internal EnumReferenceVariable(string key, BigEndianReader reader) : base(key)
    {
        Name = reader.ReadUnicodeString().String!;
        
        var classIdLength = reader.ReadInt32();
        ClassId = reader.ReadFixedLengthString(classIdLength > 0 ? classIdLength : 4);
        
        var typeIdLength = reader.ReadInt32();
        TypeId = reader.ReadFixedLengthString(typeIdLength > 0 ? typeIdLength : 4);
        
        var enumLength = reader.ReadInt32();
        Enum = reader.ReadFixedLengthString(enumLength > 0 ? enumLength : 4);
    }
}