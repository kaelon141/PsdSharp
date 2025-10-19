using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class ReferenceVariable : Variable
{
    public IReadOnlyCollection<Variable> Variables { get; set; }
    
    internal ReferenceVariable(string key, BigEndianReader reader) : base(key)
    {
        var numberOfItems = reader.ReadInt32();
        var variables = new List<Variable>();
        
        for (var i = 0; i < numberOfItems; i++)
        {
            var osTypeKey = reader.ReadFixedLengthString(4);
            
            variables.Add(Variable.Read(osTypeKey, reader));
        }
        
        Variables = variables;
    }

    public override object Value => Variables;
}