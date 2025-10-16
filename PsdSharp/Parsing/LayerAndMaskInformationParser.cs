using PsdSharp.Images;

namespace PsdSharp.Parsing;

internal static class LayerAndMaskInformationParser
{
    public static (List<Layer> Layers, GlobalLayerMaskInfo GlobalLayerMaskInfo, List<TaggedBlock> TaggedBlocks) Parse(ParseContext ctx, PsdHeader header)
    {
        var sectionLength = ctx.Traits.ReadLenN();

        if (sectionLength == 0)
        {
            return ([], new GlobalLayerMaskInfo(), []);
        }
        
        var layers = ParseLayerInfo(ctx, header);
        var globalLayerMaskInfo = ParseGlobalLayerMaskInfo(ctx);
        var taggedBlocks = new List<TaggedBlock>();
        ParseTaggedBlocks(ctx, taggedBlocks, padding: 4);

        return (layers, globalLayerMaskInfo, taggedBlocks);
    }

    private static List<Layer> ParseLayerInfo(ParseContext ctx, PsdHeader header)
    {
        var layerInfoLength = ctx.Traits.ReadLenN();
        var endPos = ctx.Reader.Position + (int)layerInfoLength;
        
        if(layerInfoLength == 0) return new List<Layer>();
        
        var layerCount = Math.Abs(ctx.Reader.ReadInt16());

        var layers = new Dictionary<uint, Layer>();
        var channelInfos = new Dictionary<uint, (short ChannelId, long DataLength)[]>();
        for (var i = 0u; i < layerCount; i++)
        {
            var (layer, channelInfo) = ParseLayerRecord(ctx);
            layers.Add(i, layer);
            channelInfos.Add(i, channelInfo);
        }

        for (var i = 0u; i < layerCount; i++)
        {
            var layer = layers[i];
            var channelInfo = channelInfos[i];
            layer.ImageData = new PerChannelImageData(ctx, header, layer.Bounds, channelInfo);
        }
        
        //Photoshop sometimes aligns the layer info to 4 bytes, sometimes doesn't... so we need to verify the endPos
        //and skip ahead manually if needed.
        if (ctx.Reader.Position < endPos)
        {
            var bytesToSkip = endPos - ctx.Reader.Position;
            ctx.Reader.Skip(bytesToSkip);
        }

        return layers.Values.ToList();
    }

    private static (Layer, (short Channelid, long DataLength)[]) ParseLayerRecord(ParseContext ctx)
    {
        var rectTop = ctx.Reader.ReadInt32();
        var rectLeft = ctx.Reader.ReadInt32();
        var rectBottom = ctx.Reader.ReadInt32();
        var rectRight = ctx.Reader.ReadInt32();
        var numChannels = ctx.Reader.ReadUInt16();
        
        var channelInfo = new Dictionary<uint, (short ChannelId, long DataLength)>();
        for (uint i = 0; i < numChannels; i++)
        {
            var channelId = ctx.Reader.ReadInt16();
            var dataLength = ctx.Traits.ReadLenN();
            channelInfo[i] = (channelId, (long)dataLength);
        }

        var signature = ctx.Reader.ReadSignature();
        if (signature != "8BIM") throw ParserUtils.DataCorrupt();

        var blendModeKey = ctx.Reader.ReadSignature();
        var blendMode = BlendModeKey.ByKey.TryGetValue(blendModeKey, out var value)
            ? value
            : throw ParserUtils.DataCorrupt();

        var opacity = ctx.Reader.ReadByte();
        var clipping = ctx.Reader.ReadByte() > 0;

        var flags = new LayerFlags(ctx.Reader.ReadByte());
        
        //filler(zero)
        ctx.Reader.ReadByte();

        var layer = new Layer
        {
            Bounds = new Rectangle(
                topLeft: new Point(rectLeft, rectTop),
                bottomRight: new Point(rectRight - 1, rectBottom - 1)
            ),
            BlendMode = blendMode,
            Opacity = opacity,
            Clipping = clipping,
            Flags = flags,
        };
        
        ParseExtraLayerData(ctx, ref layer);
        return (layer, channelInfo.Values.ToArray());
    }

    private static void ParseExtraLayerData(ParseContext ctx, ref Layer layer)
    {
        var extraDataLength = ctx.Reader.ReadUInt32();
        if (extraDataLength == 0) return;
        
        ParseMaskData(ctx, ref layer);
        ParseBlendingRanges(ctx, ref layer);
        layer.Name = ctx.Reader.ReadPascalString(4).String!;
        ParseTaggedBlocks(ctx, layer.TaggedBlocks);
    }

    private static void ParseMaskData(ParseContext ctx, ref Layer layer)
    {
        var maskDataLength = ctx.Reader.ReadUInt32();
        if(maskDataLength == 0) return;
        
        var top = ctx.Reader.ReadInt32();
        var left = ctx.Reader.ReadInt32();
        var bottom = ctx.Reader.ReadInt32();
        var right = ctx.Reader.ReadInt32();

        var defaultColor = ctx.Reader.ReadByte();
        var flags = new MaskFlags(ctx.Reader.ReadByte());

        // Order based on tests. The specification is messed up in this part.
        MaskFlags? realFlags = maskDataLength >= 36 ? new MaskFlags(ctx.Reader.ReadByte()) : null;
        byte? realDefaultColor = maskDataLength >= 36 ? ctx.Reader.ReadByte() : null;
        int? realTop = maskDataLength >= 36 ? ctx.Reader.ReadInt32() : null;
        int? realLeft = maskDataLength >= 36 ? ctx.Reader.ReadInt32() : null;
        int? realBottom = maskDataLength >= 36 ? ctx.Reader.ReadInt32() : null; 
        int? realRight = maskDataLength >= 36 ? ctx.Reader.ReadInt32() : null;

        var maskParameters = new MaskParameters();
        MaskParametersFlags parametersFlags = flags.MaskParametersPresent 
            ? new MaskParametersFlags(ctx.Reader.ReadByte())
            : new MaskParametersFlags();

        if (flags.MaskParametersPresent && parametersFlags.UserMaskDensityPresent)
        {
            maskParameters.UserMaskDensity = ctx.Reader.ReadByte();
        }

        if (flags.MaskParametersPresent && parametersFlags.UserMaskFeatherPresent)
        {
            maskParameters.UserMaskFeather = ctx.Reader.ReadDouble();
        }

        if (flags.MaskParametersPresent && parametersFlags.VectorMaskDensityPresent)
        {
            maskParameters.VectorMaskDensity = ctx.Reader.ReadByte();
        }
        
        if (flags.MaskParametersPresent && parametersFlags.VectorMaskFeatherPresent)
        {
            maskParameters.VectorMaskFeather = ctx.Reader.ReadDouble();
        }

        if (maskDataLength == 20)
        {
            //padding, only present if size = 20
            ctx.Reader.Skip(2);
        }

        layer.LayerMaskData = new LayerMaskData(flags, parametersFlags, maskParameters)
        {
            Bounds = new Rectangle(
                topLeft: new Point(left, top),
                bottomRight: new Point(right, bottom)
            ),
            DefaultColor = defaultColor,
            Flags = flags,
            RealFlags = realFlags,
            RealDefaultColor = realDefaultColor,
            RealBounds = realTop.HasValue
                ? new Rectangle(
                    topLeft: new Point(realLeft!.Value, realTop.Value),
                    bottomRight: new Point(realRight!.Value, realBottom!.Value)
                )
                : null,
        };
    }

    private static void ParseBlendingRanges(ParseContext ctx, ref Layer layer)
    {
        var blendingRangesLength = ctx.Reader.ReadUInt32();
        if(blendingRangesLength < 8) return;

        var blendingRangesData = new LayerBlendingRangesData
        {
            CompositeGreyBlendSource = ctx.Reader.ReadUInt32(),
            CompositeGreyBlendDestination = ctx.Reader.ReadUInt32()
        };

        for (var i = 8u; i < blendingRangesLength; i += 8)
        {
            var source = ctx.Reader.ReadUInt32();
            var destination = ctx.Reader.ReadUInt32();
            blendingRangesData.ChannelBlendingRanges.Add((source, destination));
        }
        
        layer.BlendingRangesData = blendingRangesData;
    }

    private static void ParseTaggedBlocks(ParseContext ctx, List<TaggedBlock> taggedBlocks, byte padding = 1)
    {
        do
        {
            var signature = ctx.Reader.ReadSignature();
            if (signature != "8BIM" && signature != "8B64")
            {
                ctx.Reader.Backtrack(ctx.StringEncoding.GetBytes(signature));
                return;
            }

            var key = ctx.Reader.ReadSignature();
            var parsedKey = AdditionalLayerInfoKey.ByKey.TryGetValue(key, out var value)
                ? value
                : new AdditionalLayerInfoKey(key, key, 4);

            var dataLength = ctx.Traits.PsdFileType == PsdFileType.Psd
                ? ctx.Reader.ReadUInt32()
                : parsedKey.PsbLengthCountSizeBytes == 4
                    ? ctx.Reader.ReadUInt32()
                    : ctx.Reader.ReadUInt64();

            var data = new byte[dataLength];
            ctx.Reader.ReadIntoBuffer(data);

            var paddingLength = dataLength % padding;
            if (paddingLength > 0)
            {
                ctx.Reader.Skip(unchecked((int)paddingLength));
            }

            taggedBlocks.Add(new TaggedBlock
            {
                Key = parsedKey,
                RawData = data,
            });
        } while (true);
    }

    private static GlobalLayerMaskInfo ParseGlobalLayerMaskInfo(ParseContext ctx)
    {
        var sectionLength = ctx.Reader.ReadUInt32();
        if (sectionLength == 0) return new GlobalLayerMaskInfo();
        if(sectionLength < 13) throw ParserUtils.DataCorrupt();

        var globalLayerMaskInfo = new GlobalLayerMaskInfo
        {
            OverlayColorSpace = ctx.Reader.ReadUInt16(),
            ColorComponent1 = ctx.Reader.ReadUInt16(),
            ColorComponent2 = ctx.Reader.ReadUInt16(),
            ColorComponent3 = ctx.Reader.ReadUInt16(),
            ColorComponent4 = ctx.Reader.ReadUInt16(),
            Opacity = ctx.Reader.ReadUInt16(),
            Kind = (GlobalLayerMaskInfo.KindEnum)ctx.Reader.ReadByte()
        };
        
        if (sectionLength > 13)
        {
            ctx.Reader.Skip(unchecked((int)sectionLength - 13));
        }
        
        return globalLayerMaskInfo;
    }
}