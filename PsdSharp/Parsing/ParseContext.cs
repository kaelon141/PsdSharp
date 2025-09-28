using System.Text;

namespace PsdSharp.Parsing;

internal class ParseContext
{
    public BigEndianReader Reader { get; }
    public FormatTraits Traits { get; private set; }
    public PsdLoadOptions PsdLoadOptions { get; }
    public Encoding StringEncoding => PsdLoadOptions.StringEncoding;
    public ColorMode ColorMode { get; private set; } = ColorMode.Rgb;

    public ParseContext(BigEndianReader reader, PsdLoadOptions psdLoadOptions)
    {
        Reader = reader;
        PsdLoadOptions = psdLoadOptions;
        Traits = default;
    }

    public void InitPsdFileType(PsdFileType psdFileType)
    {
        if (Traits.Equals(default))
        {
            Traits = FormatTraits.FromPsdFileType(psdFileType, this);
        }
    }

    public void InitColorMode(ColorMode colorMode)
    {
        if (ColorMode.Equals(null))
        {
            ColorMode = colorMode;
        }
    }
}

internal readonly record struct FormatTraits
{
    public PsdFileType PsdFileType { get; }
    public int RleCountSizeBytes { get; }
    public Func<ulong> ReadLenN { get; }
    
    public static FormatTraits FromPsdFileType(PsdFileType psdFileType, ParseContext ctx)
        => new(psdFileType, ctx);

    private FormatTraits(PsdFileType psdFileType, ParseContext ctx)
    {
        PsdFileType = psdFileType;
        RleCountSizeBytes = PsdFileType is PsdFileType.Psb ? 4 : 2;
        ReadLenN = PsdFileType is PsdFileType.Psb 
            ? () => ctx.Reader.ReadUInt64()
            : () => ctx.Reader.ReadUInt32();
    }
}