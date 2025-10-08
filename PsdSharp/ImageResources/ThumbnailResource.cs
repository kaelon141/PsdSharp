using System.Buffers.Binary;

namespace PsdSharp.ImageResources;

public class ThumbnailResource : ImageResource
{
    public enum FormatEnum
    {
        KRawRgb = 0,
        KJpegRgb = 1
    }
    
    public ThumbnailResource(string? name, byte[] rawData)
    {
        Id = ImageResourceId.ThumbnailResource;
        Name = name;
        RawData = rawData;

        Format = (FormatEnum)BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(0, 4));
        WidthPixels = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(4, 4));
        HeightPixels = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(8, 4));
        WidthBytes = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(12, 4));
        TotalSize = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(16, 4));
        SizeAfterCompression = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(20, 4));
        BitsPerPixel = BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(24, 2));
        NumberOfPlanes = BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(26, 2));
        JiffData = rawData.AsMemory(28);
    }

    public ThumbnailResource()
    {
    }
    
    public FormatEnum Format { get; set; }
    public int WidthPixels { get; set; }
    public int HeightPixels { get; set; }
    public int WidthBytes { get; set; }
    public int TotalSize { get; set; }
    public int SizeAfterCompression { get; set; }
    public short BitsPerPixel { get; set; } = 24;
    public short NumberOfPlanes { get; set; } = 1;
    public ReadOnlyMemory<byte> JiffData { get; set; }
}