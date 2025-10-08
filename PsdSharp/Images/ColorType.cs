namespace PsdSharp.Images;

public class ColorType
{
    internal ChannelInfo[] Channels { get; }

    internal ColorType(ChannelInfo[] channels)
    {
        Channels = channels;
    }

    /// <summary>
    /// Red, green, blue, alpha, 8 bits each. 32 bits per pixel
    /// </summary>
    public static ColorType Rgba8888 = new([
        new(Channel.Red, 8),
        new(Channel.Green, 8),
        new(Channel.Blue, 8),
        new(Channel.Alpha, 8),
    ]);

    /// <summary>
    /// Red, green, blue, 8 bits each, followed by 8 bits of padding to align to 32 bits.
    /// </summary>
    public static ColorType Rgb888x => new([
        new(Channel.Red, 8),
        new(Channel.Green,8),
        new(Channel.Blue, 8),
        new(Channel.Discard, 8)
    ]);
    
    /// <summary>
    /// Blue, green, red, alpha, 8 bits each. 32 bits per pixel
    /// </summary>
    public static ColorType Bgra8888 = new([
        new(Channel.Blue, 8),
        new(Channel.Green, 8),
        new(Channel.Red, 8),
        new(Channel.Alpha, 8),
    ]);
}

internal struct ChannelInfo(Channel channel, int pixelCount)
{
    public Channel Channel => channel;
    public int PixelCount => pixelCount;
}

internal enum Channel
{
    Red,
    Green,
    Blue,
    Alpha,
    Discard
}