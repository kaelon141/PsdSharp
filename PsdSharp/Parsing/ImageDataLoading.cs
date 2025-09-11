namespace PsdSharp.Parsing;

/// <summary>
/// How to handle the loading of image data. Various options are available to give you control on whether to prioritise RAM usage or performance.
/// </summary>
public enum ImageDataLoading
{
    /// <summary>
    /// Automatically decide how to handle loading of image data, based on image size.
    /// </summary>
    /// <remarks> In this mode, image data will always be loadable, and files will always be able to be written to an output stream.</remarks>
    Auto = 0,
    
    /// <summary>
    /// Load all image data into RAM immediately. Not recommended for large files.
    /// </summary>
    /// <remarks>Uses more RAM, but ensures that image data is readily available and that the file can be saved to an output stream.</remarks>
    LoadImmediately = 1,
    
    /// <summary>
    /// When the image data is encountered, buffer it to a temporary file on disk.
    /// </summary>
    /// <remarks>This is useful for large files, or for reading from network streams while keeping RAM usage low. The image data will be loaded into RAM when requested.</remarks>
    CacheOnDisk = 2,
    
    /// <summary>
    /// Load the image data only when requested. Requires a seekable stream.
    /// </summary>
    /// <remarks>If the stream turns out to be non-seekable, an exception will be thrown when attempting to retrieve image data or when attempting to save the file.</remarks>
    LoadOnDemand = 3,
    
    /// <summary>
    /// Skip image data altogether.
    /// </summary>
    /// <remarks>Useful when only reading file metadata. But note: requesting image data or trying to save the file will result in an exception.</remarks>
    Skip = 4
}