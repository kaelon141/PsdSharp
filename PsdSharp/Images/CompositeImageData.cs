using System.Buffers.Binary;
using PsdSharp.Parsing;

namespace PsdSharp.Images;

internal class CompositeImageData : ImageData
{
    private readonly PsdHeader _header;
    private readonly ImageDataSource? _dataSource;
    private readonly List<ChannelData> _loadedChannels = new();
    
    public CompositeImageData(ParseContext ctx, PsdHeader header) : base(ctx.PsdLoadOptions.ImageDataLoading)
    {
        _header = header;
        
        switch (ImageDataLoading)
        {
            case ImageDataLoading.Skip:
                return;
            case ImageDataLoading.Auto:
            {
                var approximateFileSizeBytes = 1L * header.NumberOfChannels * header.WidthInPixels * header.HeightInPixels * header.ChannelDepth / 8;
                ImageDataLoading = approximateFileSizeBytes > ctx.PsdLoadOptions.AutoImageLoadingDiskCacheThresholdBytes 
                    ? ImageDataLoading.CacheOnDisk
                    : ImageDataLoading.LoadImmediately;
                break;
            }
        }

        switch(ImageDataLoading) {
            case ImageDataLoading.LoadOnDemand:
                _dataSource = new SeekableStreamSource(ctx.Reader);
                return;
            case ImageDataLoading.CacheOnDisk:
                _dataSource = new TempFileSource(ctx.Reader);
                return;
            case ImageDataLoading.LoadImmediately:
                _dataSource = new BufferedSource(ctx.Reader);
                return;
        }
    }

    public override ushort NumberOfChannels => _header.NumberOfChannels;

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

        var compression = (ImageCompression)reader.ReadUInt16();
        if (compression == ImageCompression.Rle)
        {
            foreach (var channel in StitchChannelsFromRleData(reader))
            {
                _loadedChannels.Add(channel);
                yield return channel;
            }
            yield break;
        }

        var data = compression == ImageCompression.Raw
            ? StreamToByteArray(dataStream)
            : DecompressZipData(dataStream, compression);
        
        foreach (var channel in SplitDataIntoChannels(data))
        {
            _loadedChannels.Add(channel);
            yield return channel;
        }
    }

    public override uint Width => _header.WidthInPixels;
    public override uint Height => _header.HeightInPixels;
    public override ColorMode ColorMode => _header.ColorMode;
    public override byte[] ColorModeData => _header.ColorModeData;
    public override ushort ChannelDepth => _header.ChannelDepth;

    private byte[] StreamToByteArray(Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private byte[] DecompressZipData(Stream stream, ImageCompression compression)
    {
        return Compression.Compression.Decompress(stream, compression, _header.WidthInPixels, _header.HeightInPixels,
            _header.ChannelDepth, _header.PsdFileType == PsdFileType.Psb);
    }

    private IEnumerable<ChannelData> SplitDataIntoChannels(byte[] data)
    {
        var lengthPerChannel = data.Length / _header.NumberOfChannels;
        var bounds = new Rectangle(
            topLeft: new Point(0, 0),
            bottomRight: new Point(
                (int)_header.WidthInPixels - 1,
                (int)_header.HeightInPixels - 1
            ));
            
        for (short i = 0; i < _header.NumberOfChannels; i += 1)
        {
            var buffer = new byte[lengthPerChannel];
            Array.Copy(data, i * lengthPerChannel, buffer, 0, lengthPerChannel);
            var channel = new ChannelData(i, ImageCompression.Raw, bounds, _header.ChannelDepth, buffer, _header.PsdFileType == PsdFileType.Psb);
            _loadedChannels.Add(channel);
            yield return channel;
        }
    }
    
    private IEnumerable<ChannelData> StitchChannelsFromRleData(BigEndianReader reader)
    {
        var isPsb = _header.PsdFileType == PsdFileType.Psb;
        var rowCounts = new Dictionary<short, List<int>>();
        
        //row counts come first
        for (short i = 0; i < _header.NumberOfChannels; i += 1)
        {
            rowCounts.Add(i, []);
            for (var row = 0; row < _header.HeightInPixels; row += 1)
            {
                if(isPsb)
                    rowCounts[i].Add((int)reader.ReadUInt32());
                else
                    rowCounts[i].Add(reader.ReadUInt16());
            }
        }
        
        //now we've got to the data and can stitch the channels together.
        var bounds = new Rectangle(
            topLeft: new Point(0, 0),
            bottomRight: new Point(
                (int)_header.WidthInPixels - 1,
                (int)_header.HeightInPixels - 1
            ));
        
        for (short i = 0; i < _header.NumberOfChannels; i += 1)
        {
            var rowCountsForChannel = rowCounts[i];
            var dataLength = rowCountsForChannel.Sum();

            var dataBuffer = new byte[dataLength];
            reader.ReadIntoBuffer(dataBuffer);
            
            var byteCountLength = isPsb ? 4 : 2;
            var channelDataLength = (_header.HeightInPixels * byteCountLength) + dataLength;
            var buffer = new byte[channelDataLength];
            for (var row = 0; row < _header.HeightInPixels; row += 1)
            {
                if(isPsb)
                    BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(row * byteCountLength), (uint)rowCountsForChannel[row]);
                else
                    BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(row * byteCountLength), (ushort)rowCountsForChannel[row]);
            }
            
            Array.Copy(dataBuffer, 0, buffer, _header.HeightInPixels * byteCountLength, dataLength);
            
            var channel = new ChannelData(i, ImageCompression.Rle, bounds, _header.ChannelDepth, buffer, isPsb);
            _loadedChannels.Add(channel);
            yield return channel;
        }
    }
}