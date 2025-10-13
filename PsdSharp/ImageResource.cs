namespace PsdSharp;

public class ImageResource
{
    public ImageResourceId Id { get; set;  }
    public string? Name { get; set; } = null!;
    public byte[] RawData { get; set; } = null!;
}