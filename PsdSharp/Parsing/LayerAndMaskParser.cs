namespace PsdSharp.Parsing;

internal class LayerAndMaskParser
{
    public static LayerAndMask Parse(ParseContext ctx)
    {
        var sectionLength = ctx.Traits.ReadLenN(ctx.Reader);
        
    }

    private static   ParseLayerInfo(ParseContext ctx)
    {
        var sectionLength = ctx.Traits.ReadLenN(ctx.Reader);
        if (sectionLength % 2 == 1) sectionLength++;

        var layerCount = ctx.Reader.ReadInt16();
        
    }
}