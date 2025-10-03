using PsdSharp.Compression;
using PsdSharp.Parsing;

namespace PsdSharp.Images;

public abstract class ImageData
{
    protected ImageDataLoading ImageDataLoading;

    protected ImageData(ImageDataLoading imageDataLoading)
    {
        ImageDataLoading = imageDataLoading;
    }

    public abstract IEnumerable<ChannelData> GetChannels();

    public abstract uint Width { get; }
    public abstract uint Height { get; }
    public abstract ColorMode ColorMode { get; }

    public class ChannelData
    {
        private short _channelId;
        private readonly ImageCompression _imageCompression;
        private readonly Rectangle _bounds;
        private readonly byte _channelDepth;
        private readonly byte[] _data;
        private readonly bool _isPsb;

        public ChannelData(short channelId,
            ImageCompression imageCompression,
            Rectangle bounds,
            byte channelDepth,
            byte[] data,
            bool isPsb)
        {
            _channelId = channelId;
            _imageCompression = imageCompression;
            _bounds = bounds;
            _channelDepth = channelDepth;
            _data = data;
            _isPsb = isPsb;
        }

        public byte[] GetData()
        {
            return Compression.Compression.Decompress(_data, _imageCompression, _bounds.Width, _bounds.Height, _channelDepth, _isPsb);
        }
    }
}