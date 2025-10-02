using PsdSharp.Images;

namespace PsdSharp.Parsing;

internal class PsdParser(ParseContext context)
{
    public PsdFile Parse()
    {
        var header = HeaderParser.Parse(context);
        var colorModeData = ColorModeDataParser.Parse(context);
        var imageResources = ImageResourcesParser.Parse(context);
        var layerAndMaskInfo = LayerAndMaskInformationParser.Parse(context, header);
        
        return new PsdFile(header)
        {
            ColorModeData = colorModeData,
            ImageResources = imageResources,
            Layers = layerAndMaskInfo.Layers.AsReadOnly(),
            GlobalLayerMaskInfo = layerAndMaskInfo.GlobalLayerMaskInfo,
            TaggedBlocks = layerAndMaskInfo.TaggedBlocks.AsReadOnly(),
            ImageData = new CompositeImageData(context, header),
        };
    }
}