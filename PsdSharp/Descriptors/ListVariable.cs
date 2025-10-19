using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class ListVariable : Variable
{
    public IReadOnlyCollection<Variable> Variables { get; }

    internal ListVariable(string key, BigEndianReader reader) : base(key)
    {
        var numberOfItems = reader.ReadUInt32();

        var variables = new List<Variable>();
        for (var i = 0; i < numberOfItems; i++)
        {
            variables.Add(Variable.Read(i.ToString(), reader));
        }
    
        Variables = variables.AsReadOnly();
    }

    public override object Value => Variables;
}