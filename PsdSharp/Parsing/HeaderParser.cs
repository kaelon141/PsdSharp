using System.Data;

namespace PsdSharp.Parsing;

internal static class HeaderParser
{
    public static PsdHeader Parse(ParseContext ctx)
    {
        var signature = ctx.Reader.ReadSignature();
        if (signature != "8BPS") throw ParserUtils.DataCorrupt();

        var version = ctx.Reader.ReadUInt16();
        var psdFileType = version == 2 ? PsdFileType.Psb : PsdFileType.Psd;
        ctx.InitPsdFileType(psdFileType);
        
        //6 reserved bytes to discard
        ctx.Reader.Skip(6);
        
        var numChannels = ctx.Reader.ReadUInt16();
        var heightPx = ctx.Reader.ReadUInt32();
        if (heightPx > 300_000 || ctx.Traits.PsdFileType == PsdFileType.Psd && heightPx > 30_000)
            throw ParserUtils.DataCorrupt();
        
        var widthPx = ctx.Reader.ReadUInt32();
        if (widthPx > 300_000 || ctx.Traits.PsdFileType == PsdFileType.Psd && widthPx > 30_000)
            throw ParserUtils.DataCorrupt();

        var depth = ctx.Reader.ReadUInt16();
        if (depth is not 1 and not 8 and not 16 and not 32)
            throw ParserUtils.DataCorrupt();

        var colorMode = ctx.Reader.ReadUInt16();
        if(!Enum.IsDefined(typeof(ColorMode), colorMode)) throw ParserUtils.DataCorrupt();

        return new PsdHeader
        {
            PsdFileType = psdFileType,
            NumberOfChannels = numChannels,
            HeightInPixels = heightPx,
            WidthInPixels = widthPx,
            ChannelDepth = checked((byte)depth),
            ColorMode = (ColorMode)colorMode,
        };
    }
}