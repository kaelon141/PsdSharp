using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class EffectsLayer : TaggedBlock
{
    public short Version { get; }

    public CommonStateInfo CommonState { get; set; } = new();
    public ShadowInfo DropShadow { get; set; } = new();
    public ShadowInfo InnerShadow { get; set; } = new();
    public OuterGlowInfo OuterGlow { get; set; } = new();
    public InnerGlowInfo InnerGlow { get; set; } = new();
    public BevelInfo Bevel { get; set; } = new();
    public SolidFillInfo SolidFill { get; set; } = new();

    public EffectsLayer(byte[] rawData) : base(AdditionalLayerInfoKey.EffectsLayer, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        Version = reader.ReadInt16();
        var effectsCount = reader.ReadInt16();

        for (var i = 0; i < effectsCount; i++)
        {
            var signature = reader.ReadSignature();
            if(signature != "8BIM")
            {
                throw new InvalidDataException($"Invalid effects layer signature: {signature}");
            }
            
            var key = reader.ReadFixedLengthString(4);
            switch (key)
            {
                case "cmnS":
                    CommonState = new CommonStateInfo(reader);
                    break;
                case "dsdw":
                    DropShadow = new ShadowInfo(reader);
                    break;
                case "isdw":
                    InnerShadow = new ShadowInfo(reader);
                    break;
                case "oglw":
                    OuterGlow = new OuterGlowInfo(reader);
                    break;
                case "iglw":
                    InnerGlow = new InnerGlowInfo(reader);
                    break;
                case "bevl":
                    Bevel = new BevelInfo(reader);
                    break;
                case "sofi":
                    SolidFill = new SolidFillInfo(reader);
                    break;
            }
        }
    }


    public class CommonStateInfo
    {
        public bool IsVisible { get; set; } = true;

        public CommonStateInfo()
        {
        }

        internal CommonStateInfo(BigEndianReader reader)
        {
            reader.Skip(8);
            IsVisible = reader.ReadByte() != 0;
            reader.Skip(2);
        }
    }

    public class ShadowInfo
    {
        public int BlurValuePixels { get; set; }
        public int IntensityPercent { get; set; }
        public int AngleDegrees { get; set; }
        public int DistancePixels { get; set; }
        public PsdColor PsdColor { get; set; }
        public (string Signature, string Key) BlendMode { get; set; }
        public bool IsEnabled { get; set; }
        public bool UseAngleInAllLayerEffects { get; set; }
        public double Opacity { get; set; }
        public PsdColor? NativeColor { get; set; }

        public ShadowInfo()
        {
        }

        internal ShadowInfo(BigEndianReader reader)
        {
            reader.Skip(4);
            var version = reader.ReadInt32();

            BlurValuePixels = reader.ReadInt32();
            IntensityPercent = reader.ReadInt32();
            AngleDegrees = reader.ReadInt32();
            DistancePixels = reader.ReadInt32();
            PsdColor = PsdColor.FromPhotoshopData(reader);
            BlendMode = (reader.ReadSignature(), reader.ReadFixedLengthString(4));
            IsEnabled = reader.ReadByte() != 0;
            UseAngleInAllLayerEffects = reader.ReadByte() != 0;
            Opacity = Convert.ToDouble(reader.ReadByte()) / 255;

            if (version > 0)
            {
                NativeColor = PsdColor.FromPhotoshopData(reader);
            }
        }
    }

    public class OuterGlowInfo
    {
        public int BlurValuePixels { get; set; }
        public int IntensityPercent { get; set; }
        public PsdColor PsdColor { get; set; }
        public (string Signature, string Key) BlendMode { get; set; }
        public bool IsEnabled { get; set; }
        public double Opacity { get; set; }
        public PsdColor? NativeColor { get; set; }

        public OuterGlowInfo()
        {
        }

        internal OuterGlowInfo(BigEndianReader reader)
        {
            reader.Skip(4);
            var version = reader.ReadInt32();

            BlurValuePixels = reader.ReadInt32();
            IntensityPercent = reader.ReadInt32();
            PsdColor = PsdColor.FromPhotoshopData(reader);
            BlendMode = (reader.ReadSignature(), reader.ReadFixedLengthString(4));
            IsEnabled = reader.ReadByte() != 0;
            Opacity = Convert.ToDouble(reader.ReadByte()) / 255;

            if (version > 0)
            {
                NativeColor = PsdColor.FromPhotoshopData(reader);
            }
        }
    }

    public class InnerGlowInfo : OuterGlowInfo
    {
        public bool? Invert { get; set; }
        
        public InnerGlowInfo()
        {
        }

        internal InnerGlowInfo(BigEndianReader reader)
        {
            reader.Skip(4);
            var version = reader.ReadInt32();

            BlurValuePixels = reader.ReadInt32();
            IntensityPercent = reader.ReadInt32();
            PsdColor = PsdColor.FromPhotoshopData(reader);
            BlendMode = (reader.ReadSignature(), reader.ReadFixedLengthString(4));
            IsEnabled = reader.ReadByte() != 0;
            Opacity = Convert.ToDouble(reader.ReadByte()) / 255;

            if (version > 0)
            {
                Invert = reader.ReadByte() != 0;
                NativeColor = PsdColor.FromPhotoshopData(reader);
            }
        }
     }

    public class BevelInfo
    {
        public enum DirectionEnum : byte
        {
            Up = 0,
            Down = 1
        }
        
        public int AngleDegrees { get; set; }
        public int DepthInPixels { get; set; }
        public int BlurValuePixels { get; set; }
        public (string Signature, string Key) HighlightBlendMode { get; set; }
        public (string Signature, string Key) ShadowBlendMode { get; set; }
        public PsdColor HighlightPsdColor { get; set; }
        public PsdColor ShadowPsdColor { get; set; }
        public byte BevelStyle { get; set; }
        public double HighlightOpacity { get; set; }
        public double ShadowOpacity { get; set; }
        public bool IsEnabled { get; set; }
        public bool UseAngleInAllLayerEffects { get; set; }
        public DirectionEnum Direction { get; set; }
        public PsdColor? RealHighlightColor { get; set; }
        public PsdColor? RealShadowColor { get; set; }
        
        public BevelInfo()
        {
        }

        internal BevelInfo(BigEndianReader reader)
        {
            reader.Skip(4);
            var version = reader.ReadInt32();

            AngleDegrees = reader.ReadInt32();
            DepthInPixels = reader.ReadInt32();
            BlurValuePixels = reader.ReadInt32();
            HighlightBlendMode = (reader.ReadSignature(), reader.ReadFixedLengthString(4));
            ShadowBlendMode = (reader.ReadSignature(), reader.ReadFixedLengthString(4));
            HighlightPsdColor = PsdColor.FromPhotoshopData(reader);
            ShadowPsdColor = PsdColor.FromPhotoshopData(reader);
            BevelStyle = reader.ReadByte();
            HighlightOpacity = Convert.ToDouble(reader.ReadByte()) / 255;
            ShadowOpacity = Convert.ToDouble(reader.ReadByte()) / 255;
            IsEnabled = reader.ReadByte() != 0;
            UseAngleInAllLayerEffects = reader.ReadByte() != 0;
            Direction = (DirectionEnum)reader.ReadByte();

            if (version > 0)
            {
                RealHighlightColor = PsdColor.FromPhotoshopData(reader);
                RealShadowColor = PsdColor.FromPhotoshopData(reader);
            }
        }
    }

    public class SolidFillInfo
    {
        public string BlendModeKey { get; set; } = string.Empty;
        public PsdColor PsdColor { get; set; }
        public byte Opaciy { get; set; }
        public bool IsEnabled { get; set; }
        public PsdColor NativePsdColor { get; set; }

        public SolidFillInfo()
        {
        }

        internal SolidFillInfo(BigEndianReader reader)
        {
            reader.Skip(8);

            BlendModeKey = reader.ReadFixedLengthString(4);
            PsdColor = PsdColor.FromPhotoshopData(reader);
            Opaciy = reader.ReadByte();
            IsEnabled = reader.ReadByte() != 0;
            NativePsdColor = PsdColor.FromPhotoshopData(reader);
        }
    }
}