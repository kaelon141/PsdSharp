using PsdSharp.Images;

namespace PsdSharp.Parsing;

internal class PsdParser(ParseContext context)
{
    public PsdFile Parse()
    {
        var header = HeaderParser.Parse(context);
        var imageResources = ImageResourcesParser.Parse(context);
        var layerAndMaskInfo = LayerAndMaskInformationParser.Parse(context, header);
        var rootLayers = ParseLayerTree(layerAndMaskInfo.Layers);
        
        return new PsdFile(header)
        {
            ImageResources = imageResources,
            Layers = layerAndMaskInfo.Layers.AsReadOnly(),
            RootLayers = rootLayers,
            GlobalLayerMaskInfo = layerAndMaskInfo.GlobalLayerMaskInfo,
            TaggedBlocks = layerAndMaskInfo.TaggedBlocks,
            ImageData = new CompositeImageData(context, header),
        };
    }

    private List<Layer> ParseLayerTree(List<Layer> layers)
    {
        const string groupStartName = "</Layer group>";
        
        var rootLayers = new List<Layer>();
        Layer? currentGroup = null;

        foreach (var layer in layers)
        {
            if (layer.Bounds.Height != 0 && currentGroup is not null)
            {
                currentGroup.AddChild(layer);
                continue;
            }

            if (layer.Bounds.Height != 0 && currentGroup is null)
            {
                rootLayers.Add(layer);
                continue;
            }

            if (layer.Name == groupStartName)
            {
                layer.Parent = currentGroup;
                currentGroup = layer;
                continue;
            }
            
            if(currentGroup is null)
                continue;

            layer.Parent = currentGroup.Parent;
            foreach (var child in currentGroup.Children)
            {
                layer.AddChild(child);
            }

            if (currentGroup.Parent is null)
                rootLayers.Add(layer);
            else
                currentGroup.Parent.AddChild(layer);

            currentGroup = currentGroup.Parent;
        }
        
        return rootLayers;
    }
}