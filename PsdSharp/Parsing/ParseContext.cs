using System.Text;

namespace PsdSharp.Parsing;

internal class ParseContext
{
    public BigEndianReader Reader { get; }
    public FormatTraits Traits { get; private set; }
    public Encoding PascalStringEncoding { get; }
    public PsdLoadOptions PsdLoadOptions { get; }

    public ParseContext(BigEndianReader reader, Encoding pascalStringEncoding, PsdLoadOptions psdLoadOptions)
    {
        Reader = reader;
        PascalStringEncoding = pascalStringEncoding;
        PsdLoadOptions = psdLoadOptions;
        Traits = default;
    }

    public void InitPsdFileType(PsdFileType psdFileType)
    {
        if (Traits.Equals(default))
        {
            Traits = FormatTraits.FromPsdFileType(psdFileType);
        }
    }
}

internal readonly record struct FormatTraits
{
    public PsdFileType PsdFileType { get; }
    public int RleCountSizeBytes { get; }
    public Func<BigEndianReader, ulong> ReadLenN { get; }
    
    public static FormatTraits FromPsdFileType(PsdFileType psdFileType)
        => new(psdFileType);

    private FormatTraits(PsdFileType psdFileType)
    {
        PsdFileType = psdFileType;
        RleCountSizeBytes = PsdFileType is PsdFileType.Psb ? 4 : 2;
        ReadLenN = PsdFileType is PsdFileType.Psb 
            ? static r => r.ReadUInt64()
            : static r => r.ReadUInt32();
    }
}