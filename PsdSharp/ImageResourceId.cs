namespace PsdSharp;

public enum ImageResourceId : ushort
{
    MacPrintManagerPrintInfo = 1001,
    ResolutionInfo = 1005,
    AlphaChannelNames = 1006,
    Caption = 1008,
    BorderInfo = 1009,
    BackgroundColor = 1010,
    PrintFlags = 1011,
    GrayscaleOrMultichannelHalftoningInfo = 1012,
    ColorHalftoningInfo = 1013,
    DuotoneHalftoningInfo = 1014,
    GrayscaleOrMultichannelTransferFunction = 1015,
    ColorTransferFunctions = 1016,
    DuotoneTransferFunctions = 1017,
    DuotoneImageInfo = 1018,
    EffectiveBlackAndWhiteValuesForDotRange = 1019,
    EpsOptions = 1021,
    QuickMaskInfo = 1022,
    LayerStateInfo = 1024,
    LayersGroupInfo = 1026,
    IptcNaaRecord = 1028,
    ImageModeForRawFormatFiles = 1029,
    JpegQuality = 1030,
    GridAndGuidesInfo = 1032,
    ThumbnailResourcePhotoshopV4Only = 1033,
    CopyrightFlag = 1034,
    Url = 1035,
    ThumbnailResource = 1036,
    GlobalAngle = 1037,
    ColorsSamplerResourceOldFormat = 1038,
    IccProfile = 1039,
    Watermark = 1040,
    IccUntaggedProfile = 1041,
    EffectsVisible = 1042,
    SpotHalftone = 1043,
    DocumentSpecificIdsSeedNumber = 1044,
    UnicodeAlphaNames = 1045,
    IndexedColorTableCount = 1046,
    TransparencyIndex = 1047,
    GlobalAltitude = 1049,
    Slices = 1050,
    WorkflowUrl = 1051,
    JumpToXpep = 1052,
    AlphaIdentifiers = 1053,
    UrlList = 1054,
    VersionInfo = 1057,
    ExifData1 = 1058,
    ExifData3 = 1059,
    XmpMetadata = 1060,
    CaptionDigest = 1061,
    PrintScale = 1062,
    PixelAspectRatio = 1064,
    LayerComps = 1065,
    AlternateDuotoneColors = 1066,
    AlternateSpotColors = 1067,
    LayerSelectionIds = 1069,
    HdrToningInfo = 1070,
    PrintInfoPhotoshopCs2 = 1071,
    LayerGroupEnabledIds = 1072,
    ColorSamplersResource = 1073,
    MeasurementScale = 1074,
    TimelineInfo = 1075,
    SheetDisclosure = 1076,
    DisplayInfo = 1077,
    OnionSkins = 1078,
    CountInfo = 1080,
    PrintInfoPhotoshopCs5 = 1082,
    PrintStyle = 1083,
    MacNsPrintInfo = 1084,
    WindowsDevMode = 1085,
    AutoSaveFilePath = 1086,
    AutoSaveFormat = 1087,
    PathSelectionState = 1088,
    ClippingPathName = 2999,
    OriginPathInfo = 3000,
    ImageReadyVariables = 7000,
    ImageReadyDatasets = 7001,
    ImageReadyDefaultSelectedState = 7002,
    ImageReady7RolloverExpandedState = 7003,
    ImageReadyRolloverExpandedState = 7004,
    ImageReadySaveLayerSettings = 7005,
    ImageReadyVersion = 7006,
    LightroomWorkflow = 8000,
    PrintFlagsInfo = 10_000
}

public static class ImageResourceIdExtensions
{
    public static bool IsPathRecord(this ImageResourceId imageResourceId)
    {
        var id = (int)imageResourceId;
        return id is >= 2000 and <= 2997;
    }

    public static bool IsPluginResource(this ImageResourceId imageResourceId)
    {
        var id = (int)imageResourceId;
        return id is >= 4000 and <= 4999;
    }

    public static bool IsValidResourceId(this ImageResourceId imageResourceId)
    {
        return Enum.IsDefined(imageResourceId) || IsPathRecord(imageResourceId) || IsPluginResource(imageResourceId);
    }
}