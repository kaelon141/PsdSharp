namespace PsdSharp.Parsing;

internal class PsdParser(ParseContext context)
{
    public PsdFile Parse()
    {
        var header = HeaderParser.Parse(context);
        var colorModeData = ColorModeDataParser.Parse(context);
        var imageResources = ImageResourcesParser.Parse(context);
        var layerAndMaskInfo = LayerAndMaskInformationParser.Parse(context);
        
        return new PsdFile(header)
        {
            ColorModeData = colorModeData,
            ImageResources = imageResources,
            Layers = layerAndMaskInfo.Layers.AsReadOnly(),
            GlobalLayerMaskInfo = layerAndMaskInfo.GlobalLayerMaskInfo,
            TaggedBlocks = layerAndMaskInfo.TaggedBlocks.AsReadOnly(),
            ImageData = CreateImageData(context, header),
        };
    }

    private static ImageData CreateImageData(ParseContext ctx, PsdHeader header)
    {
        return new ImageData(ctx, header.NumberOfChannels);
    }
}