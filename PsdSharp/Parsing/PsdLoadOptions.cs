using System.Text;

namespace PsdSharp.Parsing;

public sealed class PsdLoadOptions
{
    /// <summary>
    /// Whether to leave the input stream open when the PSD file is disposed. Defaults to false.
    /// </summary>
    public bool LeaveInputOpen { get; set; } = true;

    /// <summary>
    /// Which encoding to use when reading strings. Defaults to latin1 encoding. You should only change this when using a non-latin alphabet, like Traditional Chinese.
    /// </summary>
    public Encoding? StringEncoding { get; set; } = null;
    
    /// <summary>
    /// How to handle the loading of image data. Various options are available to give you control on whether to prioritise RAM usage or performance.
    /// </summary>
    public ImageDataLoading ImageDataLoading { get; set; } = ImageDataLoading.Auto;

    /// <summary>
    /// Only used when ImageDataLoading is in Auto mode. Threshold size from which image data should be cached to disk.
    /// </summary>
    /// <remarks>By default, ImageDataLoading.Auto will load image data buffers into RAM when they're no more than 64MB in size. When over 64MB, the image data will be written to temporary files on disk instead.</remarks>
    public long AutoImageLoadingDiskCacheThresholdBytes { get; set; } = 64L * 1024 * 1024;
}