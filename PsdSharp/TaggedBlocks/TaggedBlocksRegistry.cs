namespace PsdSharp.TaggedBlocks;

internal static class TaggedBlocksRegistry
{
    public static Dictionary<AdditionalLayerInfoKey, Type> TaggedBlockTypes { get; } = new()
    {
        { AdditionalLayerInfoKey.AnimationEffects, typeof(AnimationEffects) },
        { AdditionalLayerInfoKey.Artboard1, typeof(ArtboardData) },
        { AdditionalLayerInfoKey.Artboard2, typeof(ArtboardData) },
        { AdditionalLayerInfoKey.Artboard3, typeof(ArtboardData) },
        { AdditionalLayerInfoKey.BlackWhite, typeof(BlackWhite) },
        { AdditionalLayerInfoKey.BrightnessAndContract, typeof(BrightnessAndContract) },
        { AdditionalLayerInfoKey.ColorLookup, typeof(ColorLookup) },
        { AdditionalLayerInfoKey.CompositorInfo, typeof(CompositorInfo) },
        { AdditionalLayerInfoKey.ContentGeneratorExtraData, typeof(ContentGeneratorExtraData) },
        { AdditionalLayerInfoKey.Exposure, typeof(Exposure) },
        { AdditionalLayerInfoKey.FilterMask, typeof(FilterMask) },
        { AdditionalLayerInfoKey.LayerId, typeof(LayerId) },
        { AdditionalLayerInfoKey.LayerVersion, typeof(LayerVersion) },
        { AdditionalLayerInfoKey.ObjectsBasedEffectsLayerInfo, typeof(ObjectsBasedEffectsLayerInfo) },
        { AdditionalLayerInfoKey.PixelSourceDataCc, typeof(PixelSourceDataCc) },
        { AdditionalLayerInfoKey.PixelSourceDataCc2015, typeof(PixelSourceDataCc2015) },
        { AdditionalLayerInfoKey.PlacedLayer1, typeof(PlacedLayer) },
        { AdditionalLayerInfoKey.PlacedLayer2, typeof(PlacedLayer) },
        { AdditionalLayerInfoKey.SmartObjectLayerData1, typeof(PlacedLayerData) },
        { AdditionalLayerInfoKey.SmartObjectLayerData2, typeof(SmartObjectLayerData) },
        { AdditionalLayerInfoKey.TextEngineData, typeof(TextEngineData) },
        { AdditionalLayerInfoKey.UnicodePathName, typeof(UnicodePathName) },
        { AdditionalLayerInfoKey.UserMask, typeof(UserMask) },
        { AdditionalLayerInfoKey.Vibrance, typeof(Vibrance) },
        { AdditionalLayerInfoKey.VectorOriginationData, typeof(VectorOriginationData) },
        { AdditionalLayerInfoKey.VectorStrokeData, typeof(VectorStrokeData) },
        { AdditionalLayerInfoKey.VectorStrokeContentData, typeof(VectorStrokeContentData) },
        { AdditionalLayerInfoKey.UsingAlignedRendering, typeof(UsingAlignedRendering) },
    };

    private static Dictionary<Type, AdditionalLayerInfoKey>? _keyByLayer;
    
    public static Dictionary<Type, AdditionalLayerInfoKey> KeyByLayer
    {
        get
        {
            if (_keyByLayer is not null)
                return _keyByLayer;
            
            _keyByLayer = TaggedBlockTypes.GroupBy(x => x.Value)
                .Where(x => x.Count() == 1)
                .ToDictionary(x => x.Key, x => x.First().Key);
             return _keyByLayer;
        }
    }
}