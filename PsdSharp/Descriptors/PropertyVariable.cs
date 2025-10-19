using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class PropertyVariable : Variable
{
    public readonly string Name;
    public readonly string ClassId;
    public readonly string KeyId;
    public override object Value => (Name, ClassId, KeyId);
    
    internal PropertyVariable(string key, BigEndianReader reader) : base(key)
    {
        Name = reader.ReadUnicodeString().String!;
        
        var classIdLength = reader.ReadInt32();
        ClassId = reader.ReadFixedLengthString(classIdLength > 0 ? classIdLength : 4);
        
        var keyIdLength = reader.ReadInt32();
        KeyId = reader.ReadFixedLengthString(keyIdLength > 0 ? keyIdLength : 4);
    }
}