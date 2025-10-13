using System.Buffers.Binary;
using System.Text;

namespace PsdSharp.ImageResources;

public class VersionInfo : ImageResource
{
    public VersionInfo(string? name, byte[] rawData)
    {
        Id = ImageResourceId.ThumbnailResource;
        Name = name;
        RawData = rawData;

        Version = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(0, 4));
        HasRealMergedData = rawData[4] != 0;
        
        var writerName = ReadUnicodeString(rawData.AsSpan(5));
        var readerName = ReadUnicodeString(rawData.AsSpan(5 + writerName.BytesRead));
        WriterName = writerName.String;
        ReaderName = readerName.String;
        
        FileVersion = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(5 + writerName.BytesRead + readerName.BytesRead, 4));
    }

    public VersionInfo()
    {
    }

    public int Version { get; set; }
    public bool HasRealMergedData { get; set; } = false;
    public string? WriterName { get; set; }
    public string? ReaderName { get; set; }
    public int FileVersion { get; set; }

    private (string? String, int BytesRead) ReadUnicodeString(Span<byte> data)
    {
        var amountOfCodeUnits = BinaryPrimitives.ReadInt32BigEndian(data);
        if (amountOfCodeUnits == 0) return (null, 4);

        var byteCount = amountOfCodeUnits * UnicodeEncoding.CharSize;
        var bytes = data.Slice(4, byteCount);
        
        #if NET6_0_OR_GREATER
        return (Encoding.BigEndianUnicode.GetString(bytes), byteCount + 4);
        #else
        return (Encoding.BigEndianUnicode.GetString(bytes.ToArray()), byteCount + 4);
        #endif
    } 
}