using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class AliasVariable : Variable
{
    public readonly byte[] FsSpecOrPath;
    public override object Value => FsSpecOrPath;

    internal AliasVariable(string key, BigEndianReader reader) : base(key)
    {
        var length = reader.ReadUInt32();
        
        FsSpecOrPath = new byte[length];
        reader.ReadIntoBuffer(FsSpecOrPath);
    }
}