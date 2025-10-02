namespace PsdSharp.Tests.Parsing;

public class ImageResourcesParserTests
{
    [Fact]
    public void Can_Parse_ImageResources()
    {
        var stream = PsdTestUtils.GetAsset("test.psd");
        var psdFile = PsdFile.Open(stream);
        
        Assert.Equal(9, psdFile.ImageResources.Count);
    }
}