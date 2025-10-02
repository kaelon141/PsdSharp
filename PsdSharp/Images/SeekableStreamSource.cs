using PsdSharp.Parsing;

namespace PsdSharp.Images;

internal class SeekableStreamSource : ImageDataSource
{
    private readonly BigEndianReader _reader;
    private readonly long _startPosition;
    private readonly long? _dataLength;

    private MemoryStream? _stream;
    
    public SeekableStreamSource(BigEndianReader reader, long? dataLength = null)
    {
        if (!reader.CanSeek)
        {
            throw new NotSupportedException("Cannot use the 'ImageDataLoading.LoadOnDemand' option when the input stream is non-seekable. Consider using 'ImageDataLoading.CacheOnDisk' instead.");
        }
        
        _reader = reader;
        _startPosition = reader.Position;
        _dataLength = dataLength;
        
        if(_dataLength.HasValue)
            _reader.Skip(_dataLength.Value);
    }

    public override Stream GetData()
    {
        if (_stream is not null)
            return _stream;
        
        _reader.Seek(_startPosition);

        if (_dataLength.HasValue)
        {
            var buffer = new byte[_dataLength.Value];
            _stream = new MemoryStream(buffer);

            _reader.CopyTo(_stream, _dataLength.Value);
            return _stream;
        }

        _stream = new MemoryStream();
        _reader.CopyTo(_stream);
        return _stream;
    }
}