using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class OffsetVariable : Variable
{
    public readonly string Name;
    public readonly string ClassId;
    public readonly int Offset;
    public override object Value => (Name, ClassId, Offset);
    
    internal OffsetVariable(string key, BigEndianReader reader) : base(key)
    {
        Name = reader.ReadUnicodeString().String!;
        
        var classIdLength = reader.ReadInt32();
        ClassId = reader.ReadFixedLengthString(classIdLength > 0 ? classIdLength : 4);
        
        Offset = reader.ReadInt32();
    }
}