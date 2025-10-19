using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class Descriptor
{
    public string Name { get; set; }
    public string ClassId { get; set; }
    
    public IReadOnlyCollection<Variable> Variables { get; set; }
    
    internal Descriptor(byte[] rawData) : this(new BigEndianReader(new MemoryStream(rawData)))
    {
    }

    internal Descriptor(BigEndianReader reader)
    {
        Name = reader.ReadUnicodeString().String!;

        var classIdLength = reader.ReadUInt32();
        ClassId = reader.ReadFixedLengthString(classIdLength > 0 ? classIdLength : 4);

        var numberOfItems = reader.ReadUInt32();

        var variables = new List<Variable>();
        for (var i = 0; i < numberOfItems; i++)
        {
            var keyLength = reader.ReadUInt32();
            var key = reader.ReadFixedLengthString(keyLength > 0 ? keyLength : 4);
            
            variables.Add(Variable.Read(key, reader));
        }
    
        Variables = variables.AsReadOnly();
    }
}