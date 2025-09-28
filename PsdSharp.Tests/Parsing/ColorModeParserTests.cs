namespace PsdSharp.Tests.Parsing;

public class ColorModeParserTests
{
    [Fact]
    public void Can_Parse_ColorModeData()
    {
        var stream = PsdTestUtils.GetAsset("test.psd");
        var psdFile = PsdFile.Open(stream);
        
        Assert.Equal(0, psdFile.ColorModeData.Length);
    }
}