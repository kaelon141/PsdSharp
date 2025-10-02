using PsdSharp.Images;

namespace PsdSharp;

public class Layer
{
    internal LayerFlags Flags { get; set; } = new();
    
    public Rectangle Bounds { get; set; }
    public required BlendModeKey BlendMode { get; set; }
    
    public byte Opacity { get; set; }
    public bool Clipping { get; set; }
    
    public bool TransparencyProtected
    {
        get => Flags.TransparencyProtected;
        set => Flags.TransparencyProtected = value;
    }

    public bool IsVisible
    {
        get => Flags.IsVisible;
        set => Flags.IsVisible = value;
    }

    public bool PixelDataIrrelevantToAppearanceOfDocument
    {
        get => Flags.PixelDataIrrelevantToAppearanceOfDocument;
        set => Flags.PixelDataIrrelevantToAppearanceOfDocument = value;
    }
    
    public LayerMaskData? LayerMaskData { get; set; }
    
    public LayerBlendingRangesData? BlendingRangesData { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public List<TaggedBlock> TaggedBlocks { get; set; } = [];
    
    public ImageData? ImageData { get; set; }
}

public class Channel
{
    private ushort ChannelId { get; init;  }
    
}

public class LayerMaskData
{
    internal LayerMaskData(MaskFlags flags, MaskParametersFlags parametersFlags, MaskParameters parameters)
    {
        Flags = flags;
        ParametersFlags = parametersFlags;
        Parameters = parameters;
    }
    
    public LayerMaskData()
    {
        Flags = new MaskFlags();
        ParametersFlags = new MaskParametersFlags();
        Parameters = new MaskParameters();
    }
    
    internal MaskFlags Flags { get; set; }
    internal MaskParametersFlags ParametersFlags { get; set; }
    internal MaskParameters Parameters { get; set; }
    
    public Rectangle Bounds { get; set; }
    public byte DefaultColor { get; set; }

    public bool PositionRelativeToLayer
    {
        get => Flags.PositionRelativeToLayer;
        set => Flags.PositionRelativeToLayer = value;
    }

    public bool LayerMaskDisabled
    {
        get => Flags.LayerMaskDisabled;
        set => Flags.LayerMaskDisabled = value;
    }

    public bool UserMaskCameFromRenderingOtherData
    {
        get => Flags.UserMaskCameFromRenderingOtherData;
        set => Flags.UserMaskCameFromRenderingOtherData = value;
    }

    public byte? UserMaskDensity
    {
        get => Flags.MaskParametersPresent && ParametersFlags.UserMaskDensityPresent ? Parameters.UserMaskDensity : null;
        set { 
            Parameters.UserMaskDensity = value;
            ParametersFlags.UserMaskDensityPresent = value is not null;
            Flags.MaskParametersPresent = !ParametersFlags.IsEmpty;
        }
    }

    public byte? VectorMaskDensity
    {
        get => Flags.MaskParametersPresent && ParametersFlags.VectorMaskDensityPresent ? Parameters.VectorMaskDensity : null;
        set
        {
            Parameters.VectorMaskDensity = value;
            ParametersFlags.VectorMaskDensityPresent = value is not null;
            Flags.MaskParametersPresent = !ParametersFlags.IsEmpty;
        }
    }

    public double? UserMaskFeather
    {
        get => Flags.MaskParametersPresent && ParametersFlags.UserMaskFeatherPresent ? Parameters.UserMaskFeather : null;
        set
        {
            Parameters.UserMaskFeather = value;
            ParametersFlags.UserMaskFeatherPresent = value is not null;
            Flags.MaskParametersPresent = !ParametersFlags.IsEmpty;
        }
    }

    public double? VectorMaskFeather
    {
        get => Flags.MaskParametersPresent && ParametersFlags.VectorMaskFeatherPresent ? Parameters.VectorMaskFeather : null;
        set
        {
            Parameters.VectorMaskFeather = value;
            ParametersFlags.VectorMaskFeatherPresent = value is not null;
            Flags.MaskParametersPresent = !ParametersFlags.IsEmpty;
        }
    }
    
    public MaskFlags? RealFlags { get; set; }
    
    public byte? RealDefaultColor { get; set; }
    public Rectangle? RealBounds { get; set; }
}

public class LayerBlendingRangesData
{
    public uint CompositeGreyBlendSource { get; set; }
    public uint CompositeGreyBlendDestination { get; set; }
    
    public List<(uint Source, uint Destination)> ChannelBlendingRanges { get; set; } = new();
}

public class LayerFlags : BitFlags
{
    internal LayerFlags(byte flags) : base(flags)
    {
    }

    internal LayerFlags() : base(0)
    {
    }
    
    public bool TransparencyProtected
    {
        get => GetBit(0);
        set => SetBit(0, value);
    }

    public bool IsVisible
    {
        get => GetBit(1);
        set => SetBit(1, value);
    }

    public bool PixelDataIrrelevantToAppearanceOfDocument
    {
        get => GetBit(3) && GetBit(4);
        set {
            SetBit(3, true);
            SetBit(4, value);
        }
    }
}

public class MaskFlags : BitFlags
{
    internal MaskFlags(byte flags) : base(flags)
    {
    }

    public MaskFlags() : base(0)
    {
    }

    public bool PositionRelativeToLayer
    {
        get => GetBit(0);
        set => SetBit(0, value);
    }

    public bool LayerMaskDisabled
    {
        get => GetBit(1);
        set => SetBit(1, value);
    }

    public bool UserMaskCameFromRenderingOtherData
    {
        get => GetBit(3);
        set => SetBit(3, value);
    }

    public bool MaskParametersPresent
    {
        get => GetBit(4);
        set => SetBit(4, value);
    }
}

internal class MaskParametersFlags : BitFlags
{
    internal MaskParametersFlags(byte flags) : base(flags)
    {
    }

    internal MaskParametersFlags() : base(0)
    {
    }

    public bool UserMaskDensityPresent
    {
        get => GetBit(0);
        set => SetBit(0, value);
    }

    public bool UserMaskFeatherPresent
    {
        get => GetBit(1);
        set => SetBit(1, value);
    }

    public bool VectorMaskDensityPresent
    {
        get => GetBit(2);
        set => SetBit(2, value);
    }

    public bool VectorMaskFeatherPresent
    {
        get => GetBit(3);
        set => SetBit(3, value);
    }
    
    public bool IsEmpty => Flags == 0;
}

internal class MaskParameters
{
    public byte? UserMaskDensity { get; set; }
    public double? UserMaskFeather { get; set; }
    public byte? VectorMaskDensity { get; set; }
    public double? VectorMaskFeather { get; set; }
}

public class TaggedBlock
{
    public AdditionalLayerInfoKey Key { get; set; }
    public byte[] RawData { get; set; }
}