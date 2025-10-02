using PsdSharp.Images;
using PsdSharp.Parsing;

namespace PsdSharp;

public class PsdFile
{
    internal PsdFile(PsdHeader header)
    {
        Header = header;
    }

    public PsdFile(PsdFileType fileType, ushort numberOfChannels, uint heightInPixels, uint widthInPixels, byte numBitsPerChannel, ColorMode colorMode)
    {
        Header = new PsdHeader()
        {
            PsdFileType = fileType,
            NumberOfChannels = numberOfChannels,
            HeightInPixels = heightInPixels,
            WidthInPixels = widthInPixels,
            ChannelDepth = numBitsPerChannel,
            ColorMode = colorMode,
        };
    }
    
    public static PsdFile Open(Stream stream, PsdLoadOptions? options = null)
    {
        options ??= new PsdLoadOptions();
        
        var reader = new BigEndianReader(stream, options.StringEncoding);
        var parserContext = new ParseContext(reader, options);
        var parser = new PsdParser(parserContext);
        var file = parser.Parse();
        
        if(!options.LeaveInputOpen)
            stream.Close();
        
        return file;
    }

    public PsdHeader Header { get; private set; }
    
    public ReadOnlyMemory<byte> ColorModeData { get; internal set; }
    
    public IReadOnlyCollection<Layer> Layers { get; internal set; } = [];
    public GlobalLayerMaskInfo? GlobalLayerMaskInfo { get; internal set; } = new();
    public IReadOnlyCollection<ImageResource> ImageResources { get; internal set; } = [];
    public IReadOnlyCollection<TaggedBlock> TaggedBlocks { get; internal set; } = [];
    public ImageData? ImageData { get; internal set; }
}