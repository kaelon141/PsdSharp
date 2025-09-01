namespace PsdSharp;

public enum PsdFileType
{
    /// <summary>
    /// Photoshop's standard native file format
    /// </summary>
    Psd = 1,
    
    /// <summary>
    /// Photoshop file format for large documents, allowing more data to be saved in a single file.
    /// </summary>
    Psb = 2
}