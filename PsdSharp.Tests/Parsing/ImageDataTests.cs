namespace PsdSharp.Tests.Parsing;

public class ImageDataTests
{
    [Fact]
    public void Can_Parse_ImageData()
    {
        var stream = PsdTestUtils.GetAsset("test.psd");
        var psdFile = PsdFile.Open(stream);
        
        var channels = psdFile.ImageData!.GetChannels().ToList();
        
        var channelData = channels.Select(c => c.GetData()).ToList();
        Assert.Equal(4, channelData.Count);
    }
}