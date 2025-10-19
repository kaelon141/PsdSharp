using PsdSharp.Descriptors;
using PsdSharp.Parsing;

namespace PsdSharp.TaggedBlocks;

public class PlacedLayer : TaggedBlock
{
    public enum PlacedLayerTypeEnum : int
    {
        Unknown = 0,
        Vector = 1,
        Raster = 2,
        ImageStack = 3
    }

    public int Version { get; }
    public string UniqueId { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int AntiAliasPolicy { get; }
    public PlacedLayerTypeEnum PlacedLayerType { get; }
    public double[] Transformation { get; }
    public int WarpVersion { get; }
    public int WarpDescriptorVersion { get; }
    public Descriptor WarpDescriptor { get; }
        
    
    public PlacedLayer(AdditionalLayerInfoKey key, byte[] rawData) : base(key, rawData)
    {
        var reader = new BigEndianReader(new MemoryStream(rawData));
        
        reader.Skip(4);
        Version = reader.ReadInt32();
        UniqueId = reader.ReadPascalString(0).String!;
        PageNumber = reader.ReadInt32();
        TotalPages = reader.ReadInt32();
        AntiAliasPolicy = reader.ReadInt32();
        PlacedLayerType = (PlacedLayerTypeEnum)reader.ReadInt32();
        
        Transformation = new double[8];
        for (var i = 0; i < 8; i++)
        {
            Transformation[i] = reader.ReadDouble();
        }
        
        WarpVersion = reader.ReadInt32();
        WarpDescriptorVersion = reader.ReadInt32();
        WarpDescriptor = new Descriptor(reader);
    }
}