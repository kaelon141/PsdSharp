using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public abstract class Variable
{
    public string Key { get; }
    public abstract object Value { get; }
    
    public override string ToString()
    {
        return $"{Key}: {Value}";
    }

    protected Variable(string key)
    {
        Key = key;
    }

    internal static Variable Read(string key, BigEndianReader reader)
    {
        var type = reader.ReadFixedLengthString(4);
        switch (type)
        {
            case "obj":
                return new ReferenceVariable(key, reader);
            case "Objc":
            case "GlbO":
                return new DescriptorVariable(key, reader);
            case "VlLs":
                return new ListVariable(key, reader);
            case "doub":
                return new DoubleVariable(key, reader);
            case "UntF":
                return new UnitFloatVariable(key, reader);
            case "TEXT":
                return new StringVariable(key, reader);
            case "enum":
                return new EnumVariable(key, reader);
            case "long":
                return new IntegerVariable(key, reader);
            case "comp":
                return new LargeIntegerVariable(key, reader);
            case "bool":
                return new BooleanVariable(key, reader);
            case "type":
            case "GlbC":
            case "Clss":
                return new ClassVariable(key, reader);
            case "alis":
                return new AliasVariable(key, reader);
            case "tdta":
                return new RawDataVariable(key, reader);
            case "prop":
                return new PropertyVariable(key, reader);
            case "Enmr":
                return new EnumReferenceVariable(key, reader);
            case "rele":
                return new OffsetVariable(key, reader);
            case "Idnt":
                return new IdentifierVariable(key, reader);
            case "indx":
                return new IndexVariable(key, reader);
            case "name":
                return new NameVariable(key, reader);
        }

        throw new NotSupportedException($"osType key '{type}' is not recognized.");
    }
}