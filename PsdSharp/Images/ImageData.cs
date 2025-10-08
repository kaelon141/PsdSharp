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

    public abstract ushort NumberOfChannels { get; }
    public abstract IEnumerable<ChannelData> GetChannels();
    public abstract uint Width { get; }
    public abstract uint Height { get; }
    public abstract ColorMode ColorMode { get; }
    public abstract byte[] ColorModeData { get; }
    public abstract ushort ChannelDepth { get; }

    public class ChannelData
    {
        public short ChannelId;
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
            ChannelId = channelId;
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