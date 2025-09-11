namespace PsdSharp;

public class ImageResource
{
    public required ImageResourceId Id { get; init;  }
    public required string? Name { get; init; }
    public required byte[] RawData { get; set; }
}