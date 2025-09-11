namespace PsdSharp.Parsing;

internal static class ImageResourcesParser
{
    public static List<ImageResource> Parse(ParseContext ctx)
    {
        var sectionLength = ctx.Reader.ReadUInt32();
        
        var list = new List<ImageResource>();
        uint amountConsumed = 0;

        while (amountConsumed < sectionLength)
        {
            var signature = ctx.Reader.ReadSignature();
            if (signature != "8BIM") throw ParserUtils.DataCorrupt();

            var resourceId = (ImageResourceId)ctx.Reader.ReadUInt16();
            var name = ctx.Reader.ReadPascalString();
            
            var resourceDataLength = ctx.Reader.ReadUInt32();
            var dataBuffer = resourceDataLength == 0 ? Array.Empty<byte>() : new byte[resourceDataLength];
            ctx.Reader.ReadIntoBuffer(dataBuffer);
            
            list.Add(new ImageResource
            {
                Id = resourceId,
                Name = name.String,
                RawData = dataBuffer,
            });

            amountConsumed += 4 + 2 + (uint)name.NumBytesRead + 4 + unchecked((uint)dataBuffer.Length);
        }

        return list;
    }
}