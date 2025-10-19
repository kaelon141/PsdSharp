using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class DescriptorVariable : Variable
{
    public string Name { get; set; }
    public string ClassId { get; set; }
    
    public IReadOnlyCollection<Variable> Variables { get; set; }

    internal DescriptorVariable(string key, BigEndianReader reader) : base(key)
    {
        Name = reader.ReadUnicodeString().String!;

        var classIdLength = reader.ReadUInt32();
        ClassId = reader.ReadFixedLengthString(classIdLength > 0 ? classIdLength : 4);

        var numberOfItems = reader.ReadUInt32();

        var variables = new List<Variable>();
        for (var i = 0; i < numberOfItems; i++)
        {
            var keyLength = reader.ReadUInt32();
            var itemKey = reader.ReadFixedLengthString(keyLength > 0 ? keyLength : 4);
            
            variables.Add(Variable.Read(itemKey, reader));
        }
    
        Variables = variables.AsReadOnly();
    }

    public override object Value => Variables;
}