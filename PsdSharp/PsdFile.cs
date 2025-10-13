using System.Text;
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
        #if !NET6_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        #endif
        
        options ??= new PsdLoadOptions();

        
        if (options.StringEncoding is null)
        {
            #if !NET6_0_OR_GREATER
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                options.StringEncoding = Encoding.GetEncoding("ISO-8859-1");
            #else
                options.StringEncoding = Encoding.Latin1;
            #endif
        }
        
        var reader = new BigEndianReader(stream, options.StringEncoding, options.LeaveInputOpen);
        var parserContext = new ParseContext(reader, options);
        var parser = new PsdParser(parserContext);
        var file = parser.Parse();
        
        if(!options.LeaveInputOpen)
            stream.Close();
        
        return file;
    }

    public PsdHeader Header { get; private set; }
    
    public IReadOnlyCollection<Layer> Layers { get; internal set; } = [];
    public GlobalLayerMaskInfo? GlobalLayerMaskInfo { get; internal set; } = new();
    public IReadOnlyCollection<ImageResource> ImageResources { get; internal set; } = [];
    public IReadOnlyCollection<TaggedBlock> TaggedBlocks { get; internal set; } = [];
    public ImageData? ImageData { get; internal set; }
}