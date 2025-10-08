namespace PsdSharp.Parsing;

internal static class ColorModeDataParser
{
    public static byte[] Parse(ParseContext ctx)
    {
        var len = ctx.Reader.ReadUInt32();
        if (len == 0)
            return [];

        var buffer = new byte[len];
        ctx.Reader.ReadIntoBuffer(buffer);
        
        return buffer;
    }
}