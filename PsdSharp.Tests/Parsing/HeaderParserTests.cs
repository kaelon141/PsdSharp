using System.Data;
using System.Text;
using PsdSharp.Parsing;

namespace PsdSharp.Tests.Parsing;

public class HeaderParserTests
{
   private static readonly byte[] PsdHeader =
      EncodingProvider.Latin1.GetBytes("8BPS\x00\x01\x00\x00\x00\x00\x00\x00\x00\x03\x00\x00\x00\x96\x00\x00\x00\xfa\x00\x08\x00\x03\x00\x00\x00\x00");
   private static readonly byte[] PsdHeaderWithDimensionsTooLarge =
      EncodingProvider.Latin1.GetBytes("8BPS\x00\x01\x00\x00\x00\x00\x00\x00\x00\x03\x00\x00\xc3\x50\x00\x00\xc3\x50\x00\x08\x00\x03\x00\x00\x00\x00");
   private static readonly byte[] PsbHeader =
      EncodingProvider.Latin1.GetBytes("8BPS\x00\x02\x00\x00\x00\x00\x00\x00\x00\x03\x00\x00\xc3\x50\x00\x00\xc3\x50\x00\x08\x00\x03\x00\x00\x00\x00");
   private static readonly byte[] PsbHeaderWithDimensionsTooLarge =
      EncodingProvider.Latin1.GetBytes("8BPS\x00\x02\x00\x00\x00\x00\x00\x00\x00\x03\x00\x00\x06\xdd\xd0\x00\x00\x06\xdd\xd0\x00\x03\x00\x00\x00\x00");

   private ParseContext CreateParseContext(byte[] header)
   {
      var stream = new MemoryStream(header);
      var reader = new BigEndianReader(stream);
      return new ParseContext(reader, new PsdLoadOptions());
   }

   [Fact]
   public void HeaderParser_Can_Read_Header()
   {
      var header = HeaderParser.Parse(CreateParseContext(PsdHeader));
      
      Assert.Equal(PsdFileType.Psd, header.PsdFileType);
      Assert.Equal(3u, header.NumberOfChannels);
      Assert.Equal(150u, header.HeightInPixels);
      Assert.Equal(250u, header.WidthInPixels);
      Assert.Equal(8u, header.ChannelDepth);
   }

   [Fact]
   public void HeaderParser_Throws_If_DimensionsTooLargeForPsd()
   {
      var parseContext = CreateParseContext(PsdHeaderWithDimensionsTooLarge);

      Assert.Throws<DataException>(() => HeaderParser.Parse(parseContext));
   }
   
   [Fact]
   public void HeaderParser_Allows_Larger_Dimensions_For_Psb()
   {
      var parseContext = CreateParseContext(PsbHeader);
      
      var header = HeaderParser.Parse(parseContext);
      
      Assert.Equal(PsdFileType.Psb, header.PsdFileType);
      Assert.Equal(3u, header.NumberOfChannels);
      Assert.Equal(50_000u, header.HeightInPixels);
      Assert.Equal(50_000u, header.WidthInPixels);
      Assert.Equal(8u, header.ChannelDepth);
   }
   
   [Fact]
   public void HeaderParser_Throws_If_DimensionsTooLargeForPsb()
   {
      var parseContext = CreateParseContext(PsbHeaderWithDimensionsTooLarge);

      Assert.Throws<DataException>(() => HeaderParser.Parse(parseContext));
   }

   [Fact]
   public void HeaderParser_CanParse_RealWorld_File()
   {
      var stream = PsdTestUtils.GetAsset("test.psd");
      var reader = new BigEndianReader(stream);
      var parseContext = new ParseContext(reader, new PsdLoadOptions());
      
      var header = HeaderParser.Parse(parseContext);
      
      Assert.Equal(PsdFileType.Psd, header.PsdFileType);
      Assert.Equal(4u, header.NumberOfChannels);
      Assert.Equal(1024u, header.HeightInPixels);
      Assert.Equal(1024u, header.WidthInPixels);
      Assert.Equal(8u, header.ChannelDepth);
   }

   [Fact]
   public void HeaderParser_Throws_If_Not_Psd_File()
   {
      var stream = new MemoryStream(EncodingProvider.Latin1.GetBytes("PNG\x00\x01\x00"));
      var reader = new BigEndianReader(stream);
      var parseContext = new ParseContext(reader, new PsdLoadOptions());
      
      Assert.Throws<DataException>(() => HeaderParser.Parse(parseContext));
   }
   
   [Fact]
   public void HeaderParser_Throws_If_ChannelDepth_Not_PowerOfTwo()
   {
      var buffer = PsdHeader;
      PsdHeader[PsdHeader.Length - 7] = 0x06;
      var stream = new MemoryStream(buffer);
      var reader = new BigEndianReader(stream);
      var parseContext = new ParseContext(reader, new PsdLoadOptions());
      
      Assert.Throws<DataException>(() => HeaderParser.Parse(parseContext));
   }
   
   [Fact]
   public void HeaderParser_Throws_If_ColorMode_Unknown()
   {
      var buffer = PsdHeader;
      PsdHeader[PsdHeader.Length - 5] = 0x16;
      var stream = new MemoryStream(buffer);
      var reader = new BigEndianReader(stream);
      var parseContext = new ParseContext(reader, new PsdLoadOptions());
      
      Assert.Throws<DataException>(() => HeaderParser.Parse(parseContext));
   }
}