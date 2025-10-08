namespace PsdSharp.ImageResources;

internal static class ImageResourceRegistry
{
    public static Dictionary<ImageResourceId, Type> ResourceTypes { get; } = new()
    {
        { ImageResourceId.ResolutionInfo, typeof(ResolutionInfo) },
        { ImageResourceId.ThumbnailResource, typeof(ThumbnailResource) },
        { ImageResourceId.VersionInfo, typeof(VersionInfo) }
    };
}