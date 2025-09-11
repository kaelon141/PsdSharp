namespace PsdSharp.Parsing;

internal static class ColorModeDataParser
{
    public static ReadOnlyMemory<byte> Parse(ParseContext ctx)
    {
        var len = ctx.Reader.ReadUInt32();
        if (len == 0)
            return ReadOnlyMemory<byte>.Empty;

        var buffer = new byte[len];
        ctx.Reader.ReadIntoBuffer(buffer);
        
        return buffer;
    }
}