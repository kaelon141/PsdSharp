using System.Buffers.Binary;

namespace PsdSharp.ImageResources;

public class ResolutionInfo : ImageResource
{
    private const double FixedPointDivisor = 0x10000;
    
    public ResolutionInfo(string? name, byte[] rawData)
    {
        Id = ImageResourceId.ResolutionInfo;
        Name = name;
        RawData = rawData;
        
        HorizontalResolutionDPI = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(0, 4)) / FixedPointDivisor;
        HorizontalResolutionDisplayUnit = (ResolutionDisplayUnit)BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(4, 2));
        WidthDisplayUnit = (SizeDisplayUnit)BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(6, 2));
        
        VerticalResolutionDPI = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(8, 4)) / FixedPointDivisor;
        VerticalResolutionDisplayUnit = (ResolutionDisplayUnit)BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(12, 2));
        HeightDisplayUnit = (SizeDisplayUnit)BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(14, 2));
    }

    public ResolutionInfo()
    {
    }
    
    public enum ResolutionDisplayUnit : short
    {
        PixelsPerInch = 1,
        PixelsPerCentimeter = 2
    }
    
    public enum SizeDisplayUnit : short
    {
        Inches = 1,
        Centimeters = 2,
        Points = 3,
        Picas = 4,
        Columns = 5
    }
    
    /// <summary>
    /// The horizontal resolution of the image, in dots per inch.
    /// </summary>
    public double HorizontalResolutionDPI { get; set; }
    
    /// <summary>
    /// The unit in which the horizontal resolution should be displayed to the user.
    /// </summary>
    public ResolutionDisplayUnit HorizontalResolutionDisplayUnit { get; set; }
    
    /// <summary>
    /// The unit in which the image width should be displayed to the user.
    /// </summary>
    public SizeDisplayUnit WidthDisplayUnit { get; set; }
    
    /// <summary>
    /// The vertical resolution of the image, in dots per inch.
    /// </summary>
    public double VerticalResolutionDPI { get; set; }

    /// <summary>
    /// The unit in which the vertical resolution should be displayed to the user.
    /// </summary>
    public ResolutionDisplayUnit VerticalResolutionDisplayUnit { get; set; }

    /// <summary>
    /// The unit in which the image height should be displayed to the user.
    /// </summary>
    public SizeDisplayUnit HeightDisplayUnit { get; set; }
}