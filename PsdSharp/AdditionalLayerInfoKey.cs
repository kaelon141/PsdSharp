namespace PsdSharp;

public class AdditionalLayerInfoKey
{
    public readonly string Key;
    public readonly string Description;
    public readonly byte PsbLengthCountSizeBytes;
    
    internal AdditionalLayerInfoKey(string key, string description, byte psbLengthCountSizeBytes)
    {
        Key = key;
        Description = description;
        PsbLengthCountSizeBytes = psbLengthCountSizeBytes;
    }
    
    
    public static readonly AdditionalLayerInfoKey Alpha = new("Alph", nameof(Alpha), 8);
    public static readonly AdditionalLayerInfoKey AnimationEffects = new("anFX", nameof(AnimationEffects), 4);
    public static readonly AdditionalLayerInfoKey Annotations = new("Anno", nameof(Annotations), 4);
    public static readonly AdditionalLayerInfoKey Artboard1 = new("artb", nameof(Artboard1), 4);
    public static readonly AdditionalLayerInfoKey Artboard2 = new("artd", nameof(Artboard2), 4);
    public static readonly AdditionalLayerInfoKey Artboard3 = new("abdd", nameof(Artboard3), 4);
    public static readonly AdditionalLayerInfoKey BlackWhite = new("blwh", nameof(BlackWhite), 4);
    public static readonly AdditionalLayerInfoKey BlendClippingElements = new("clbl", nameof(BlendClippingElements), 4);
    public static readonly AdditionalLayerInfoKey BlendFillOpacity = new("iOpa", nameof(BlendFillOpacity), 4);
    public static readonly AdditionalLayerInfoKey BlendInteriorElements = new("infx", nameof(BlendInteriorElements), 4);
    public static readonly AdditionalLayerInfoKey BrightnessAndContract = new("brit", nameof(BrightnessAndContract), 4);
    public static readonly AdditionalLayerInfoKey ChannelBalance = new("blnc", nameof(ChannelBalance), 4);
    public static readonly AdditionalLayerInfoKey ChannelBlendingRestrictions = new("brst", nameof(ChannelBlendingRestrictions), 4);
    public static readonly AdditionalLayerInfoKey ChannelMixer = new("mixr", nameof(ChannelMixer), 4);
    public static readonly AdditionalLayerInfoKey ColorLookup = new("clrL", nameof(ColorLookup), 4);
    public static readonly AdditionalLayerInfoKey CompositorInfo = new("cinf", nameof(CompositorInfo), 4);
    public static readonly AdditionalLayerInfoKey ContentGeneratorExtraData = new("CgEd", nameof(ContentGeneratorExtraData), 4);
    public static readonly AdditionalLayerInfoKey Curves = new("curv", nameof(Curves), 4);
    public static readonly AdditionalLayerInfoKey EffectsLayer = new("lrFX", nameof(EffectsLayer), 4);
    public static readonly AdditionalLayerInfoKey ExportSetting1 = new("extd", nameof(ExportSetting1), 4);
    public static readonly AdditionalLayerInfoKey ExportSetting2= new("extn", nameof(ExportSetting2), 4);
    public static readonly AdditionalLayerInfoKey Exposure = new("expA", nameof(Exposure), 4);
    public static readonly AdditionalLayerInfoKey FilterEffects1 = new("FXid", nameof(FilterEffects1), 8);
    public static readonly AdditionalLayerInfoKey FilterEffects2 = new("FEid", nameof(FilterEffects2), 8);
    public static readonly AdditionalLayerInfoKey FilterEffects3 = new("FELS", nameof(FilterEffects3), 8);
    public static readonly AdditionalLayerInfoKey FilterMask = new("FMsk", nameof(FilterMask), 8);
    public static readonly AdditionalLayerInfoKey ForeignEffectId = new("ffxi", nameof(ForeignEffectId), 4);
    public static readonly AdditionalLayerInfoKey FramedGroup = new("frgb", nameof(FramedGroup), 4);
    public static readonly AdditionalLayerInfoKey GradientFill = new("GdFl", nameof(GradientFill), 4);
    public static readonly AdditionalLayerInfoKey GradientMap = new("grdm", nameof(GradientMap), 4);
    public static readonly AdditionalLayerInfoKey HueSaturation= new("hue2", nameof(HueSaturation), 4);
    public static readonly AdditionalLayerInfoKey HueSaturationV4= new("hue ", nameof(HueSaturationV4), 4);
    public static readonly AdditionalLayerInfoKey Invert= new("nvrt", nameof(Invert), 4);
    public static readonly AdditionalLayerInfoKey Knockout = new("knko", nameof(Knockout), 4);
    public static readonly AdditionalLayerInfoKey Layer = new("Layr", nameof(Layer), 8);
    public static readonly AdditionalLayerInfoKey Layer16 = new("Lr16", nameof(Layer16), 8);
    public static readonly AdditionalLayerInfoKey Layer32 = new("Lr32", nameof(Layer32), 8);
    public static readonly AdditionalLayerInfoKey LayerId = new("lyid", nameof(LayerId), 4);
    public static readonly AdditionalLayerInfoKey LayerMaskAsGlobalMask = new("lmgm", nameof(LayerMaskAsGlobalMask), 4);
    public static readonly AdditionalLayerInfoKey LayerNameSource = new("lnsr", nameof(LayerNameSource), 4);
    public static readonly AdditionalLayerInfoKey LayerVersion = new("lyvr", nameof(LayerVersion), 4);
    public static readonly AdditionalLayerInfoKey Levels = new("levl", nameof(LayerVersion), 4);
    public static readonly AdditionalLayerInfoKey LinkedLayer1 = new("lnkD", nameof(LinkedLayer1), 4);
    public static readonly AdditionalLayerInfoKey LinkedLayer2 = new("lnk2", nameof(LinkedLayer2), 4);
    public static readonly AdditionalLayerInfoKey LinkedLayer3 = new("lnk3", nameof(LinkedLayer3), 4);
    public static readonly AdditionalLayerInfoKey LinkedLayerExternal = new("lnkE", nameof(LinkedLayerExternal), 4);
    public static readonly AdditionalLayerInfoKey Metadata = new("shmd", nameof(Metadata), 4);
    public static readonly AdditionalLayerInfoKey NestedSectionDivider = new("lsdk", nameof(NestedSectionDivider), 4);
    public static readonly AdditionalLayerInfoKey ObjectsBasedEffectsLayerInfo = new("lfx2", nameof(ObjectsBasedEffectsLayerInfo), 4);
    public static readonly AdditionalLayerInfoKey ObjectsBasedEffectsLayerInfoV0 = new("lmfx", nameof(ObjectsBasedEffectsLayerInfoV0), 4);
    public static readonly AdditionalLayerInfoKey ObjectsBasedEffectsLayerInfoV1 = new("lfxs", nameof(ObjectsBasedEffectsLayerInfoV1), 4);
    public static readonly AdditionalLayerInfoKey Pattern = new("patt", nameof(PatternData), 4);
    public static readonly AdditionalLayerInfoKey PatternData = new("shpa", nameof(PatternData), 4);
    public static readonly AdditionalLayerInfoKey PatternFill = new("PtFl", nameof(PatternFill), 4);
    public static readonly AdditionalLayerInfoKey Patterns1 = new("Patt", nameof(Patterns1), 4);
    public static readonly AdditionalLayerInfoKey Patterns2 = new("Pat2", nameof(Patterns2), 4);
    public static readonly AdditionalLayerInfoKey Patterns3 = new("Pat3", nameof(Patterns3), 4);
    public static readonly AdditionalLayerInfoKey PhotoFilter = new("phfl", nameof(PhotoFilter), 4);
    public static readonly AdditionalLayerInfoKey PixelSourceDataCc = new("PxSc", nameof(PixelSourceDataCc), 4);
    public static readonly AdditionalLayerInfoKey PixelSourceDataCc2015 = new("PxSD", nameof(PixelSourceDataCc2015), 8);
    public static readonly AdditionalLayerInfoKey PlacedLayer1 = new("plLd", nameof(PlacedLayer1), 4);
    public static readonly AdditionalLayerInfoKey PlacedLayer2 = new("PlLd", nameof(PlacedLayer2), 4);
    public static readonly AdditionalLayerInfoKey Posterize = new("post", nameof(Posterize), 4);
    public static readonly AdditionalLayerInfoKey Protected = new("lspf", nameof(Protected), 4);
    public static readonly AdditionalLayerInfoKey ReferencePoint = new("fxrp", nameof(ReferencePoint), 4);
    public static readonly AdditionalLayerInfoKey SavingMergedTransparency = new("Mtrn", nameof(SavingMergedTransparency), 8);
    public static readonly AdditionalLayerInfoKey SavingMergedTransparency16 = new("Mt16", nameof(SavingMergedTransparency16), 8);
    public static readonly AdditionalLayerInfoKey SavingMergedTransparency32 = new("Mt32", nameof(SavingMergedTransparency32), 8);
    public static readonly AdditionalLayerInfoKey SectionDivider = new("lsct", nameof(SectionDivider), 4);
    public static readonly AdditionalLayerInfoKey SelectiveColor = new("selc", nameof(SelectiveColor), 4);
    public static readonly AdditionalLayerInfoKey SheetColor = new("lclr", nameof(SheetColor), 4);
    public static readonly AdditionalLayerInfoKey SmartObjectLayerData1 = new("soLD", nameof(SmartObjectLayerData1), 4);
    public static readonly AdditionalLayerInfoKey SmartObjectLayerData2 = new("SoLE", nameof(SmartObjectLayerData2), 4);
    public static readonly AdditionalLayerInfoKey SolidColorSheet = new("SoCo", nameof(SolidColorSheet), 4);
    public static readonly AdditionalLayerInfoKey TextEngineData = new("Txt2", nameof(TextEngineData), 4);
    public static readonly AdditionalLayerInfoKey Threshold = new("thrs", nameof(Threshold), 4);
    public static readonly AdditionalLayerInfoKey TransparencyShapesLayer = new("tsly", nameof(TransparencyShapesLayer), 4);
    public static readonly AdditionalLayerInfoKey TypeToolInfo = new("tySh", nameof(TypeToolInfo), 4);
    public static readonly AdditionalLayerInfoKey TypeToolObject = new("TySh", nameof(TypeToolObject), 4);
    public static readonly AdditionalLayerInfoKey UnicodeLayerName = new("luni", nameof(UnicodeLayerName), 4);
    public static readonly AdditionalLayerInfoKey UnicodePathName = new("pths", nameof(UnicodePathName), 4);
    public static readonly AdditionalLayerInfoKey UserMask = new("LMsk", nameof(UserMask), 8);
    public static readonly AdditionalLayerInfoKey UsingAlignedRendering = new("sn2P", nameof(UsingAlignedRendering), 4);
    public static readonly AdditionalLayerInfoKey VectorMask = new("vmsk", nameof(VectorMask), 4);
    public static readonly AdditionalLayerInfoKey VectorMaskAsGlobalmask = new("vmgm", nameof(VectorMaskAsGlobalmask), 4);
    public static readonly AdditionalLayerInfoKey VectorMaskPhotoshop6 = new("vsms", nameof(VectorMaskPhotoshop6), 4);
    public static readonly AdditionalLayerInfoKey VectorOriginationData = new("vogk", nameof(VectorOriginationData), 4);
    public static readonly AdditionalLayerInfoKey VectorOriginationUnknown= new("vowv", nameof(VectorOriginationUnknown), 4);
    public static readonly AdditionalLayerInfoKey VectorStrokeContentData = new("vscg", nameof(VectorStrokeContentData), 4);
    public static readonly AdditionalLayerInfoKey VectorStrokeData = new("vstk", nameof(VectorStrokeData), 4);
    public static readonly AdditionalLayerInfoKey Vibrance = new("vibA", nameof(Vibrance), 4);
    
    // Unknown tags
    public static readonly AdditionalLayerInfoKey Cai = new("CAI ", nameof(Cai), 4);
    public static readonly AdditionalLayerInfoKey Geni = new("GenI", nameof(Geni), 4);
    public static readonly AdditionalLayerInfoKey Ocio = new("OCIO", nameof(Ocio), 4);
    
    public static readonly AdditionalLayerInfoKey[] All = [Alpha, AnimationEffects, Annotations, Artboard1, Artboard2, Artboard3,
        BlackWhite, BlendClippingElements, BlendFillOpacity, BlendInteriorElements, BrightnessAndContract,
        ChannelBalance, ChannelBlendingRestrictions, ChannelMixer, ColorLookup, CompositorInfo,
        ContentGeneratorExtraData, Curves, EffectsLayer, ExportSetting1, ExportSetting2,
        Exposure, FilterEffects1, FilterEffects2, FilterEffects3, FilterMask,
        ForeignEffectId, FramedGroup, GradientFill, GradientMap, HueSaturation, HueSaturationV4,
        Invert, Knockout, Layer, Layer16, Layer32,
        LayerId, LayerMaskAsGlobalMask, LayerNameSource, LayerVersion, Levels,
        LinkedLayer1, LinkedLayer2, LinkedLayer3, LinkedLayerExternal, Metadata,
        NestedSectionDivider, ObjectsBasedEffectsLayerInfo, ObjectsBasedEffectsLayerInfoV0, ObjectsBasedEffectsLayerInfoV1, Pattern,
        PatternData, PatternFill, Patterns1, Patterns2,
        Patterns3, PhotoFilter, PixelSourceDataCc, PixelSourceDataCc2015, PlacedLayer1,
        PlacedLayer2, Posterize, Protected, ReferencePoint,
        SavingMergedTransparency, SavingMergedTransparency16, SavingMergedTransparency32, SectionDivider, SelectiveColor,
        SheetColor, SmartObjectLayerData1, SmartObjectLayerData2, SolidColorSheet,
        TextEngineData, Threshold, TransparencyShapesLayer,
        TypeToolInfo, TypeToolObject, UnicodeLayerName,
        UnicodePathName, UserMask,
        UsingAlignedRendering,
        VectorMask,
        VectorMaskAsGlobalmask,
        VectorMaskPhotoshop6,
        VectorOriginationData,
        VectorOriginationUnknown,
        VectorStrokeContentData,
        VectorStrokeData,
        Vibrance,
        
        // Unknown
        Cai,
        Geni,
        Ocio
    ];

    public static readonly IDictionary<string, AdditionalLayerInfoKey> ByKey = All.ToDictionary(k => k.Key);
}