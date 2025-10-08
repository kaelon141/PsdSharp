using PsdSharp.Parsing;

namespace PsdSharp.Images;

internal class PerChannelImageData : ImageData
{
    private readonly PsdHeader _psdHeader;
    
    private readonly Rectangle _bounds;
    
    private readonly (short ChannelId, long DataLength)[] _channels;

    private readonly ImageDataSource? _dataSource;

    private readonly List<ChannelData> _loadedChannels = new();
    private readonly bool _isPsb;

    public PerChannelImageData(ParseContext ctx, PsdHeader header, Rectangle bounds, (short ChannelId, long DataLength)[] channels) : base(ctx.PsdLoadOptions.ImageDataLoading)
    {
        _bounds = bounds;
        _psdHeader = header;
        _channels = channels;
        _isPsb = ctx.Traits.PsdFileType == PsdFileType.Psb;

        var dataLength = _channels
            .Aggregate(0L, (acc, info) => acc + info.DataLength);
        if (dataLength == 0) return;
        
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
    
    public override ushort NumberOfChannels => (ushort)_channels.Length;
    
    public override IEnumerable<ChannelData> GetChannels()
    {
        if (_loadedChannels.Count > 0)
        {
            foreach (var channel in _loadedChannels.Where(x => x.ChannelId >= 0))
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

            var channelData = new ChannelData(channelInfo.ChannelId, compression, _bounds, _psdHeader.ChannelDepth, buffer, _isPsb);
            _loadedChannels.Add(channelData);

            if (channelData.ChannelId < 0)
                continue;
            
            yield return channelData;
        }
    }

    public override uint Width => _bounds.Width;
    public override uint Height => _bounds.Height;
    public override ColorMode ColorMode => _psdHeader.ColorMode;
    public override byte[] ColorModeData => _psdHeader.ColorModeData;
    public override ushort ChannelDepth => _psdHeader.ChannelDepth;
}