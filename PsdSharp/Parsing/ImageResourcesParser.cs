using PsdSharp.ImageResources;

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
            var name = ctx.Reader.ReadPascalString(alignmentSize: 2);
            
            var resourceDataLength = ctx.Reader.ReadUInt32();
            if (resourceDataLength % 2 == 1)
            {
                resourceDataLength++;
            }
            
            var dataBuffer = resourceDataLength == 0 ? Array.Empty<byte>() : new byte[resourceDataLength];
            ctx.Reader.ReadIntoBuffer(dataBuffer);

            if (ImageResourceRegistry.ResourceTypes.TryGetValue(resourceId, out var type))
            {
                var imageResource = (ImageResource)Activator.CreateInstance(type, name.String, dataBuffer)!;
                list.Add(imageResource);
            }
            else
            {
                list.Add(new ImageResource
                {
                    Id = resourceId,
                    Name = name.String,
                    RawData = dataBuffer,
                });
            }

            amountConsumed += 4 + 2 + (uint)name.NumBytesRead + 4 + unchecked((uint)dataBuffer.Length);
        }

        return list;
    }
}