using PsdSharp.Parsing;

namespace PsdSharp.Images;

internal class TempFileSource : ImageDataSource
{
    private readonly FileInfo _tempFile;
    
    public TempFileSource(BigEndianReader reader, long? dataLength = null)
    {
        var tempFilePath = Path.GetTempFileName();
        _tempFile = new FileInfo(tempFilePath);
        
        var fileStream = _tempFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        if (dataLength.HasValue)
            reader.CopyTo(fileStream, dataLength.Value);
        else
            reader.CopyTo(fileStream);
        
        fileStream.Close();
    }
    
    public override Stream GetData()
    {
        return _tempFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}