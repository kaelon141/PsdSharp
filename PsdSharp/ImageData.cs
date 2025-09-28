using PsdSharp.Parsing;

namespace PsdSharp;

public class ImageData
{
    internal ImageData(ParseContext ctx, ushort numChannels)
    {
        _numChannels = numChannels;
        _imageDataLoading = ctx.PsdLoadOptions.ImageDataLoading = ctx.PsdLoadOptions.ImageDataLoading;
        
        if (_imageDataLoading == ImageDataLoading.Skip)
        {
            return;
        }

        switch (_imageDataLoading)
        {
            case ImageDataLoading.LoadOnDemand when !ctx.Reader.CanSeek:
                throw new NotSupportedException("Cannot use the 'ImageDataLoading.LoadOnDemand' option when the input stream is non-seekable. Consider using 'ImageDataLoading.CacheOnDisk' instead.");
            case ImageDataLoading.LoadOnDemand:
                _streamPosition = ctx.Reader.Position;
                _reader = ctx.Reader;
                break;
            case ImageDataLoading.CacheOnDisk:
            case ImageDataLoading.Auto when !ctx.Reader.CanSeek:
            case ImageDataLoading.Auto when ctx.Reader.CanSeek && (ctx.Reader.Length - ctx.Reader.Position) > ctx.PsdLoadOptions.AutoImageLoadingDiskCacheThresholdBytes:
            {
                var tempFilePath = Path.GetTempFileName();
                _fileInfo = new FileInfo(tempFilePath);
            
                var fileStream = _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                ctx.Reader.CopyTo(fileStream, ctx.Reader.Length);
                fileStream.Close();
            
                return;
            }
            case ImageDataLoading.LoadImmediately or ImageDataLoading.Auto:
            {
                LoadChannels(ctx.Reader);
                break;
            }
        }
        
    }
    internal ImageData(ParseContext ctx, long dataLength, Func<(short ChannelId, long DataLength)[]> channelFetchFunc)
    {
        _channelFetchFunc = channelFetchFunc;
        _imageDataLoading = ctx.PsdLoadOptions.ImageDataLoading;
        
        if (_imageDataLoading == ImageDataLoading.Skip)
        {
            ctx.Reader.Skip(dataLength);
            return;
        }

        switch (_imageDataLoading)
        {
            case ImageDataLoading.LoadOnDemand when !ctx.Reader.CanSeek:
                throw new NotSupportedException("Cannot use the 'ImageDataLoading.LoadOnDemand' option when the input stream is non-seekable. Consider using 'ImageDataLoading.CacheOnDisk' instead.");
            case ImageDataLoading.LoadOnDemand:
                _streamPosition = ctx.Reader.Position;
                _reader = ctx.Reader;
                break;
            case ImageDataLoading.CacheOnDisk:
            case ImageDataLoading.Auto when
                dataLength > ctx.PsdLoadOptions.AutoImageLoadingDiskCacheThresholdBytes:
            {
                var tempFilePath = Path.GetTempFileName();
                _fileInfo = new FileInfo(tempFilePath);
            
                var fileStream = _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                ctx.Reader.CopyTo(fileStream, dataLength);
                fileStream.Close();
            
                return;
            }
            case ImageDataLoading.LoadImmediately or ImageDataLoading.Auto:
            {
                LoadChannels(ctx.Reader);
                break;
            }
        }
    }

    private readonly ImageDataLoading _imageDataLoading;

    private readonly ushort? _numChannels;
    private readonly Func<(short ChannelId, long DataLength)[]>? _channelFetchFunc;
    
    private readonly BigEndianReader? _reader;
    private readonly long? _streamPosition;
    
    private readonly FileInfo? _fileInfo;
    
    private Dictionary<short, ChannelData>? _cachedChannels;

    private void LoadChannels(BigEndianReader? reader = null)
    {
        
        if (_imageDataLoading == ImageDataLoading.Skip)
        {
            throw new InvalidOperationException("Cannot load channels when the ImageDataLoading option is set to 'Skip'.");
        }
        if (_imageDataLoading == ImageDataLoading.LoadOnDemand)
        {
            _reader!.Seek(_streamPosition!.Value);
            reader = _reader;
        }

        if (_fileInfo is not null)
        {
            var fileStream = _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            reader = new BigEndianReader(fileStream);
        }
        
        if(reader is null) throw new Exception("Something went wrong while attempting to load image data.");

        if (_numChannels is not null)
        {
            LoadChannelsFromEndOfFile(reader);
        }
        else
        {
            LoadChannelsFromLayer(reader);
        }
    }

    private void LoadChannelsFromLayer(BigEndianReader reader)
    {
        var channelInfos = _channelFetchFunc();
        var channels = new List<ChannelData>();
        
        foreach (var channelInfo in channelInfos)
        {
            var compression = (ImageCompression)reader.ReadUInt16();
            if (!Enum.IsDefined(compression)) throw ParserUtils.DataCorrupt();
            
            var buffer = new byte[channelInfo.DataLength - 2];
            reader.ReadIntoBuffer(buffer);

            var channelData = new ChannelData
            {
                ChannelId = channelInfo.ChannelId,
                ImageCompression = compression,
                Data = buffer,
            };
            channels.Add(channelData);
        }
        
        _cachedChannels = channels.ToDictionary(c => c.ChannelId);
    }

    private void LoadChannelsFromEndOfFile(BigEndianReader reader)
    {
        var data = reader.ReadUntilEnd();
        var channelData = new ChannelData()
        {
            ChannelId = -255,
            ImageCompression = ImageCompression.Rle,
            Data = data,
        };
        _cachedChannels = new Dictionary<short, ChannelData>
        {
            {channelData.ChannelId, channelData}
        };
    }
    
    public Dictionary<short, ChannelData> Channels
    {
        get
        {
            if (_cachedChannels == null)
                LoadChannels();
            return _cachedChannels!;
        }
    }
}

public class ChannelData
{
    public required short ChannelId { get; init; }
    public required ImageCompression ImageCompression { get; init; }
    public required byte[] Data { get; set; }
}