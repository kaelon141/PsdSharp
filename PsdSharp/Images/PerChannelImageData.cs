using PsdSharp.Parsing;

namespace PsdSharp.Images;

internal class PerChannelImageData : ImageData
{
    private readonly Rectangle _bounds;
    private readonly ColorMode _colorMode;
    private readonly byte _channelDepth;
    
    private readonly (short ChannelId, long DataLength)[] _channels;

    private readonly ImageDataSource? _dataSource;

    private readonly List<ChannelData> _loadedChannels = new();
    private readonly bool _isPsb;

    public PerChannelImageData(ParseContext ctx, Rectangle bounds, ColorMode colorMode, byte channelDepth, (short ChannelId, long DataLength)[] channels) : base(ctx.PsdLoadOptions.ImageDataLoading)
    {
        _bounds = bounds;
        _colorMode = colorMode;
        _channelDepth = channelDepth;
        _channels = channels;
        _isPsb = ctx.Traits.PsdFileType == PsdFileType.Psb;

        var dataLength = _channels.Aggregate(0L, (acc, info) => acc + info.DataLength);
        
        if (ImageDataLoading == ImageDataLoading.Skip)
        {
            ctx.Reader.Skip(dataLength);
            return;
        }

        switch (ImageDataLoading)
        {
            case ImageDataLoading.LoadOnDemand:
                _dataSource = new SeekableStreamSource(ctx.Reader, dataLength);
                return;
            case ImageDataLoading.CacheOnDisk:
            case ImageDataLoading.Auto when dataLength > ctx.PsdLoadOptions.AutoImageLoadingDiskCacheThresholdBytes:
                _dataSource = new TempFileSource(ctx.Reader, dataLength);

                return;
            case ImageDataLoading.LoadImmediately or ImageDataLoading.Auto:
                _dataSource = new BufferedSource(ctx.Reader, dataLength);
                return;
        }
    }
    
    public override IEnumerable<ChannelData> GetChannels()
    {
        if (_loadedChannels.Count > 0)
        {
            foreach (var channel in _loadedChannels)
                yield return channel;
            yield break;
        }
        
        if(_dataSource is null)
            throw new InvalidOperationException("Cannot load channels when the ImageDataLoading option is set to 'Skip'.");

        using var dataStream = _dataSource.GetData();
        var reader = new BigEndianReader(dataStream);

        foreach (var channelInfo in _channels)
        {
            var compression = (ImageCompression)reader.ReadUInt16();
            if (!Enum.IsDefined(compression)) throw ParserUtils.DataCorrupt();
            
            var buffer = new byte[channelInfo.DataLength - 2];
            reader.ReadIntoBuffer(buffer);

            var channelData = new ChannelData(channelInfo.ChannelId, compression, _bounds, _channelDepth, buffer, _isPsb);
            _loadedChannels.Add(channelData);
            yield return channelData;
        }
    }

    public override uint Width => _bounds.Width;
    public override uint Height => _bounds.Height;
    public override ColorMode ColorMode => _colorMode;
}