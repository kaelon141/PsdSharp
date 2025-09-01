namespace PsdSharp;

public class AdditionalLayerInfoKey
{
    public readonly string Key;
    public readonly string Description;
    public readonly byte PsbLengthCountSizeBytes;
    
    private AdditionalLayerInfoKey(string key, string description, byte psbLengthCountSizeBytes)
    {
        Key = key;
        Description = description;
        PsbLengthCountSizeBytes = psbLengthCountSizeBytes;
    }
    
    public static AdditionalLayerInfoKey EffectsLayer = new("lrFX", nameof(EffectsLayer), 4);
    public static AdditionalLayerInfoKey TypeToolInfo = new("tySh", nameof(TypeToolInfo), 4);
    public static AdditionalLayerInfoKey UnicodeLayerName = new("luni", nameof(UnicodeLayerName), 4);
    public static AdditionalLayerInfoKey LaserId = new("lyid", nameof(LaserId), 4);
    public static AdditionalLayerInfoKey ObjectsBasedEffectsLayerInfo = new("lfx2", nameof(ObjectsBasedEffectsLayerInfo), 4);
    public static AdditionalLayerInfoKey Patterns1 = new("Patt", nameof(Patterns1), 4);
    public static AdditionalLayerInfoKey Patterns2 = new("Pat2", nameof(Patterns2), 4);
    public static AdditionalLayerInfoKey Patterns3 = new("Pat3", nameof(Patterns3), 4);
    public static AdditionalLayerInfoKey Annotations = new("Anno", nameof(Annotations), 4);
    public static AdditionalLayerInfoKey BlendClippingElements = new("clbl", nameof(BlendClippingElements), 4);
    public static AdditionalLayerInfoKey BlendInteriorElements = new("infx", nameof(BlendInteriorElements), 4);
    public static AdditionalLayerInfoKey Knockout = new("knko", nameof(Knockout), 4);
    public static AdditionalLayerInfoKey Protected = new("lspf", nameof(Protected), 4);
    public static AdditionalLayerInfoKey SheetColor = new("lclr", nameof(SheetColor), 4);
    public static AdditionalLayerInfoKey ReferencePoint = new("fxrp", nameof(ReferencePoint), 4);
    public static AdditionalLayerInfoKey GradientSettings = new("grdm", nameof(GradientSettings), 4);
    public static AdditionalLayerInfoKey SectionDivider = new("lsct", nameof(SectionDivider), 4);
    public static AdditionalLayerInfoKey ChannelBlendingRestrictions = new("brst", nameof(ChannelBlendingRestrictions), 4);
    public static AdditionalLayerInfoKey SolidColorSheet = new("SoCo", nameof(SolidColorSheet), 4);
    public static AdditionalLayerInfoKey PatternFill = new("PtFl", nameof(PatternFill), 4);
    public static AdditionalLayerInfoKey GradientFill = new("GdFl", nameof(GradientFill), 4);
    public static AdditionalLayerInfoKey VectorMask = new("vmsk", nameof(VectorMask), 4);
    public static AdditionalLayerInfoKey VectorMaskPhotoshop6 = new("vsms", nameof(VectorMaskPhotoshop6), 4);
    public static AdditionalLayerInfoKey TypeToolObject = new("TySh", nameof(TypeToolObject), 4);
    public static AdditionalLayerInfoKey ForeignEffectId = new("ffxi", nameof(ForeignEffectId), 4);
    public static AdditionalLayerInfoKey LayerNameSource = new("lnsr", nameof(LayerNameSource), 4);
    public static AdditionalLayerInfoKey PatternData = new("shpa", nameof(PatternData), 4);
    public static AdditionalLayerInfoKey Metadata = new("shmd", nameof(Metadata), 4);
    public static AdditionalLayerInfoKey LayerVersion = new("lyvr", nameof(LayerVersion), 4);
    public static AdditionalLayerInfoKey TransparencyShapesLayer = new("tsly", nameof(TransparencyShapesLayer), 4);
    public static AdditionalLayerInfoKey LayerMaskAsGlobalMask = new("lmgm", nameof(LayerMaskAsGlobalMask), 4);
    public static AdditionalLayerInfoKey VectorMaskAsGlobalmask = new("vmgm", nameof(VectorMaskAsGlobalmask), 4);
    public static AdditionalLayerInfoKey BrightnessAndContract = new("brit", nameof(BrightnessAndContract), 4);
    public static AdditionalLayerInfoKey ChannelMixer = new("mixr", nameof(ChannelMixer), 4);
    public static AdditionalLayerInfoKey ColorLookup = new("clrL", nameof(ColorLookup), 4);
    public static AdditionalLayerInfoKey PlacedLayer = new("plLd", nameof(PlacedLayer), 4);
    public static AdditionalLayerInfoKey LinkedLayer1 = new("lnkD", nameof(LinkedLayer1), 4);
    public static AdditionalLayerInfoKey LinkedLayer2 = new("lnk2", nameof(LinkedLayer2), 4);
    public static AdditionalLayerInfoKey LinkedLayer3 = new("lnk3", nameof(LinkedLayer3), 4);
    public static AdditionalLayerInfoKey PhotoFilter = new("phfl", nameof(PhotoFilter), 4);
    public static AdditionalLayerInfoKey BlackWhite = new("blwh", nameof(BlackWhite), 4);
    public static AdditionalLayerInfoKey ContentGeneratorExtraData = new("CgEd", nameof(ContentGeneratorExtraData), 4);
    public static AdditionalLayerInfoKey TextEngineData = new("Txt2", nameof(TextEngineData), 4);
    public static AdditionalLayerInfoKey Vibrance = new("vibA", nameof(Vibrance), 4);
    public static AdditionalLayerInfoKey UnicodePathName = new("pths", nameof(UnicodePathName), 4);
    public static AdditionalLayerInfoKey AnimationEffects = new("anFX", nameof(AnimationEffects), 4);
    public static AdditionalLayerInfoKey FilterMask = new("FMsk", nameof(FilterMask), 8);
    public static AdditionalLayerInfoKey PlacedLayerData = new("soLD", nameof(PlacedLayerData), 4);
    public static AdditionalLayerInfoKey VectorStrokeData = new("vstk", nameof(VectorStrokeData), 4);
    public static AdditionalLayerInfoKey VectorStrokeContentData = new("vscg", nameof(VectorStrokeContentData), 4);
    public static AdditionalLayerInfoKey UsingAlignedRendering = new("sn2P", nameof(UsingAlignedRendering), 4);
    public static AdditionalLayerInfoKey VectorOriginationData = new("vogk", nameof(VectorOriginationData), 4);
    public static AdditionalLayerInfoKey PixelSourceDataCc = new("PxSc", nameof(PixelSourceDataCc), 4);
    public static AdditionalLayerInfoKey CompositorUsed = new("cinf", nameof(CompositorUsed), 4);
    public static AdditionalLayerInfoKey PixelSourceDataCc2015 = new("PxSD", nameof(PixelSourceDataCc2015), 8);
    public static AdditionalLayerInfoKey Artboard1 = new("artb", nameof(Artboard1), 4);
    public static AdditionalLayerInfoKey Artboard2 = new("artd", nameof(Artboard2), 4);
    public static AdditionalLayerInfoKey Artboard3 = new("abdd", nameof(Artboard3), 4);
    public static AdditionalLayerInfoKey SmartObjectLayerData = new("SoLE", nameof(SmartObjectLayerData), 4);
    public static AdditionalLayerInfoKey SavingMergedTransparency1 = new("Mtrn", nameof(SavingMergedTransparency1), 8);
    public static AdditionalLayerInfoKey SavingMergedTransparency2 = new("Mt16", nameof(SavingMergedTransparency2), 8);
    public static AdditionalLayerInfoKey SavingMergedTransparency3 = new("Mt32", nameof(SavingMergedTransparency3), 8);
    public static AdditionalLayerInfoKey UserMask = new("LMsk", nameof(UserMask), 8);
    public static AdditionalLayerInfoKey Exposure = new("expA", nameof(Exposure), 4);
    public static AdditionalLayerInfoKey FilterEffects1 = new("FXid", nameof(FilterEffects1), 8);
    public static AdditionalLayerInfoKey FilterEffects2 = new("FEid", nameof(FilterEffects2), 8);
    
    // Below keys are not documented by Adobe, save for the fact that when reading PSB files, the length MUST be read as 8 bytes.
    // Hence they're included in the code, as otherwise the reading code will not handle these correctly.
    public static AdditionalLayerInfoKey Unknown1 = new("Lr16", nameof(Unknown1), 8);
    public static AdditionalLayerInfoKey Unknown2 = new("Lr32", nameof(Unknown2), 8);
    public static AdditionalLayerInfoKey Unknown3 = new("Layr", nameof(Unknown3), 8);
    public static AdditionalLayerInfoKey Unknown4 = new("Alph", nameof(Unknown4), 8);


}