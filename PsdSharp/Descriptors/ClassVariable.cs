using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class ClassVariable : Variable
{
    public readonly string Name;
    public readonly string ClassId;
    public override object Value => (Name, ClassId);
    
    internal ClassVariable(string key, BigEndianReader reader) : base(key)
    {
        Name = reader.ReadUnicodeString().String!;
        
        var classIdLength = reader.ReadInt32();
        ClassId = reader.ReadFixedLengthString(classIdLength > 0 ? classIdLength : 4);
    }
}