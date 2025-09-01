namespace PsdSharp;

public class FileSignature
{
    public readonly PsdFileType FileType;
    public readonly string Extension;
    public readonly byte[] Signature;
    public ushort FileVersion;
    
    private FileSignature(PsdFileType fileType, string extension, byte[] signature, ushort fileVersion)
    {
        FileType = fileType;
        Extension = extension;
        Signature = signature;
    }

    public static FileSignature Psd = new(PsdFileType.Psd, ".psd", [0x38, 0x42, 0x50, 0x53], 1);
    public static FileSignature Psb = new(PsdFileType.Psb, ".psb", [0x38, 0x42, 0x50, 0x53], 2);
}