using System.Text;
using PsdSharp.Parsing;

namespace PsdSharp.Tests.Parsing
{
    public class BigEndianReaderTests
    {
        // ----- Primitives ----------------------------------------------------

        [Fact]
        public void Read_Primitives_BigEndian()
        {
            // int32 = 0x01020304, float = 1.0f as BE, double = 1.5 as BE, etc.
            var data = PsdTestUtils.Concat(
                PsdTestUtils.BE16(0x1122),
                PsdTestUtils.BE32(0x01020304),
                PsdTestUtils.BE64(0x0102030405060708UL)
            );

            using var r = new BigEndianReader(PsdTestUtils.Ms(data));

            Assert.Equal(0x1122, (ushort)r.ReadUInt16());
            Assert.Equal((uint) 0x01020304, (uint)r.ReadUInt32());
            Assert.Equal(0x0102030405060708UL, (ulong)r.ReadUInt64());
        }

        [Fact]
        public void ReadByte_And_ReadSByte_Eof_Throws()
        {
            using var r = new BigEndianReader(PsdTestUtils.Ms()); // empty stream
            Assert.Throws<EndOfStreamException>(() => r.ReadByte());
            Assert.Throws<EndOfStreamException>(() => r.ReadSByte());
        }

        // ----- Pascal strings ------------------------------------------------

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Pascal_NonEmpty_Respects_Alignment(byte alignment)
        {
            // "ABC" then a marker byte 0xEE. Reader must consume alignment bytes and land on marker.
            var pascal = PsdTestUtils.Pascal("ABC", alignment);
            var data = PsdTestUtils.Concat(pascal, [0xEE]);

            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            var s = r.ReadPascalString(alignment);

            Assert.Equal("ABC", s.String);
            Assert.Equal(0xEE, r.ReadByte()); // next byte is the marker, padding consumed
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Pascal_ZeroLength_Returns_Null_And_Consumes_Padding(byte alignment)
        {
            // zero-length pascal string should return null; reader must consume alignment-1 bytes
            var pascalNull = PsdTestUtils.PascalNull(alignment);
            var data = PsdTestUtils.Concat(pascalNull, [0x7A]);

            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            var s = r.ReadPascalString(alignment);

            Assert.Null(s.String);
            Assert.Equal(0x7A, r.ReadByte()); // landed on marker
        }

        [Fact]
        public void Pascal_MaxLength_255()
        {
            string s = new string('x', 255);
            var pascal = PsdTestUtils.Pascal(s, 4);
            using var r = new BigEndianReader(PsdTestUtils.Ms(pascal), EncodingProvider.Latin1);
            var got = r.ReadPascalString(4);
            Assert.Equal(s, got.String);
        }

        // ----- Unicode strings (UTF-16BE, length = code units) --------------

        [Fact]
        public void Unicode_Empty_Returns_Null_ByDesign()
        {
            // length (uint32 BE) = 0
            var header = PsdTestUtils.BE32(0x0);
            var payload = PsdTestUtils.Utf16Be( 0x0, 0x0);
            var data = PsdTestUtils.Concat(header, payload);
            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            var s = r.ReadUnicodeString();
            Assert.Null(s.String);
            Assert.Equal(6, s.NumBytesRead);
        }

        [Fact]
        public void Unicode_Simple_BMP_String()
        {
            // "AB" -> codeUnits=2, bytes: 00 41 00 42
            var header = PsdTestUtils.BE32(2u);
            var payload = PsdTestUtils.Utf16Be(0x0041, 0x0042, 0x0, 0x0);
            var data = PsdTestUtils.Concat(header, payload);

            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            var s = r.ReadUnicodeString();
            Assert.Equal("AB", s.String);
        }

        [Fact]
        public void Unicode_Supplementary_Uses_SurrogatePair()
        {
            // 😀 U+1F600 → surrogate pair D83D DE00
            var header = PsdTestUtils.BE32(2u); // two code units
            var payload = PsdTestUtils.Utf16Be(0xD83D, 0xDE00, 0x0, 0x0);
            var data = PsdTestUtils.Concat(header, payload);

            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            var s = r.ReadUnicodeString();
            Assert.Equal("😀", s.String);
        }

        [Fact]
        public void Unicode_Large_Length_Over_Cap_Throws()
        {
            // length = (Max + 1) code units
            const uint maxUnits = 16 * 1024 * 1024;
            var data = PsdTestUtils.BE32(maxUnits + 1);
            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            Assert.Throws<InvalidDataException>(() => r.ReadUnicodeString());
        }

        [Fact]
        public void Unicode_Eof_Inside_Payload_Throws()
        {
            // Say header says 4 code units (8 bytes) but we only provide 2 bytes
            var header = PsdTestUtils.BE32(4u);
            var payload = PsdTestUtils.Utf16Be(0x0041); // only 2 bytes provided
            var data = PsdTestUtils.Concat(header, payload);
            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            Assert.Throws<EndOfStreamException>(() => r.ReadUnicodeString());
        }

        // ----- Skip ----------------------------------------------------------

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(64)]
        [InlineData(512)]
        [InlineData(5000)]
        public void Skip_Advances_Correctly(int count)
        {
            // Create stream: [count bytes of 0xAA] then 0xCC marker
            var payload = Enumerable.Repeat((byte)0xAA, count).ToArray();
            var data = PsdTestUtils.Concat(payload, [0xCC]);

            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            r.Skip(count);
            Assert.Equal(0xCC, r.ReadByte());
        }

        [Fact]
        public void Skip_Past_End_Throws()
        {
            using var r = new BigEndianReader(PsdTestUtils.Ms(0x01, 0x02), EncodingProvider.Latin1);
            Assert.Throws<EndOfStreamException>(() => r.Skip(3));
        }

        // ----- Mixed: Pascal then primitive to prove alignment consumption ----

        [Fact]
        public void Pascal4_Followed_By_UInt16_Boundary_Is_Correct()
        {
            // Pascal("A") aligned to 4 → [01 'A' 00 00], then UInt16 BE 0x1234
            var pascal = PsdTestUtils.Pascal("A", 4);
            var u16 = PsdTestUtils.BE16(0x1234);
            var data = PsdTestUtils.Concat(pascal, u16);

            using var r = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            Assert.Equal("A", r.ReadPascalString(4).String);
            Assert.Equal(0x1234, r.ReadUInt16());
        }

        // ----- Dispose / Close ----------------------------------------------

        [Fact]
        public void Dispose_Closes_Stream_When_Not_LeaveOpen()
        {
            var ms = new MemoryStream([1, 2, 3], writable: false);
            var reader = new BigEndianReader(ms, EncodingProvider.Latin1, leaveOpen: false);
            reader.Dispose();
            Assert.Throws<ObjectDisposedException>(() => ms.ReadByte());
        }

        [Fact]
        public void Dispose_Does_Not_Close_Stream_When_LeaveOpen()
        {
            var ms = new MemoryStream([1, 2, 3], writable: false);
            var reader = new BigEndianReader(ms, EncodingProvider.Latin1, leaveOpen: true);
            reader.Dispose();
            int b = ms.ReadByte(); // should still work
            Assert.True(b >= 0);
        }
        
        [Fact]
        public void ReadUnicodeString_ConsumesTrailingNullTerminator()
        {
            // Arrange: string "Hi"
            // Count = 2 (UInt32, BE)
            // Data = 'H' (0x0048), 'i' (0x0069), encoded UTF16-BE
            // Then 0x0000 terminator (2 null bytes)
            byte[] data =
            [
                0x00, 0x00, 0x00, 0x02, // count = 2
                0x00, 0x48,             // 'H'
                0x00, 0x69,             // 'i'
                0x00, 0x00              // terminator
            ];

            using var ms = new MemoryStream(data);
            using var reader = new BigEndianReader(ms, Encoding.BigEndianUnicode);

            // Act
            var result = reader.ReadUnicodeString();

            // Assert string value
            Assert.Equal("Hi", result.String);
            Assert.Equal(10, result.NumBytesRead);

            // Assert we consumed *everything*, i.e. stream at end
            Assert.Equal(ms.Length, ms.Position);
        }
        
        [Fact]
        public void ReadUnicodeString_ZeroLength_ReturnsNullAndConsumesTerminator()
        {
            // Arrange:
            // Length = 0 (UInt32 BE)
            // Then 0x0000 terminator (two null bytes)
            byte[] data =
            {
                0x00, 0x00, 0x00, 0x00, // count = 0
                0x00, 0x00              // terminator
            };

            using var ms = new MemoryStream(data);
            using var reader = new BigEndianReader(ms, Encoding.BigEndianUnicode);

            // Act
            var result = reader.ReadUnicodeString();

            // Assert: returns null and consumed all bytes
            Assert.Null(result.String);
            Assert.Equal(6, result.NumBytesRead);
            Assert.Equal(ms.Length, ms.Position);
        }

        // ----- Backtrack ----------------------------------------------------

        [Fact]
        public void Backtrack_SingleByte_ReadByteSequence()
        {
            using var r = new BigEndianReader(PsdTestUtils.Ms(0x02, 0x03), EncodingProvider.Latin1);
            r.Backtrack([0x01]);
            Assert.Equal(0x01, r.ReadByte());
            Assert.Equal(0x02, r.ReadByte());
            Assert.Equal(0x03, r.ReadByte());
        }

        [Fact]
        public void Backtrack_MultipleBytes_ReadUInt32_Then_ReadByte()
        {
            using var r = new BigEndianReader(PsdTestUtils.Ms(0x44, 0x55), EncodingProvider.Latin1);
            r.Backtrack([0x11, 0x22, 0x33]); // internal buffer now: 11 22 33
            // ReadUInt32 should consume: 11 22 33 44 (big-endian)
            uint val = r.ReadUInt32();
            Assert.Equal(0x11223344u, val);
            // Remaining underlying stream byte 0x55
            Assert.Equal(0x55, r.ReadByte());
        }

        [Fact]
        public void Backtrack_PartialDrain_With_ReadByte_Then_ReadUInt16()
        {
            using var r = new BigEndianReader(PsdTestUtils.Ms(0x04, 0x05), EncodingProvider.Latin1);
            r.Backtrack([0x01, 0x02, 0x03]); // internal: 01 02 03
            Assert.Equal(0x01, r.ReadByte()); // remaining internal: 02 03
            ushort next = r.ReadUInt16(); // consumes 02 03
            Assert.Equal(0x0203, next);
            Assert.Equal(0x04, r.ReadByte());
            Assert.Equal(0x05, r.ReadByte());
        }

        [Fact]
        public void Backtrack_MultipleCalls_AppendOrder_Fifo()
        {
            using var r = new BigEndianReader(PsdTestUtils.Ms(0xAA), EncodingProvider.Latin1);
            r.Backtrack([0x03]); // internal: 03
            r.Backtrack([0x01, 0x02]); // internal: 03 01 02 (append semantics)
            Assert.Equal(0x03, r.ReadByte());
            Assert.Equal(0x01, r.ReadByte());
            Assert.Equal(0x02, r.ReadByte());
            Assert.Equal(0xAA, r.ReadByte());
        }
        [Fact]
        public void Read_SignedIntegers_BigEndian_NegativeValues()
        {
            // Int16: 0xFFFE => -2, Int32: 0x80000000 => int.MinValue, Int64: 0xFFFFFFFFFFFFFFFE => -2
            var data = PsdTestUtils.Concat(
                new byte[] { 0xFF, 0xFE },
                new byte[] { 0x80, 0x00, 0x00, 0x00 },
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE }
            );
            using var r = new BigEndianReader(PsdTestUtils.Ms(data));
            Assert.Equal(-2, r.ReadInt16());
            Assert.Equal(int.MinValue, r.ReadInt32());
            Assert.Equal(-2L, r.ReadInt64());
        }

        [Fact]
        public void Read_Double_BigEndian()
        {
            // Half 1.0 => 0x3C00; Single 1.0f => 0x3F800000; Double 1.5 => 0x3FF8000000000000
            var data = PsdTestUtils.Concat(
                new byte[] { 0x3F, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            );
            using var r = new BigEndianReader(PsdTestUtils.Ms(data));
            Assert.Equal(1.5, r.ReadDouble());
        }

        [Fact]
        public void ReadSignature_ReturnsAscii4_And_ThrowsOnEof()
        {
            using (var r = new BigEndianReader(PsdTestUtils.Ms((byte)'8', (byte)'B', (byte)'P', (byte)'S'), EncodingProvider.Latin1))
            {
                Assert.Equal("8BPS", r.ReadSignature());
            }

            using (var r2 = new BigEndianReader(PsdTestUtils.Ms((byte)'8', (byte)'B', (byte)'P'), EncodingProvider.Latin1))
            {
                Assert.Throws<EndOfStreamException>(() => r2.ReadSignature());
            }
        }

        [Fact]
        public void ReadIntoBuffer_FillsSpan_And_ThrowsOnShortRead()
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            using var r = new BigEndianReader(PsdTestUtils.Ms(bytes));
            Span<byte> buf = stackalloc byte[3];
            r.ReadIntoBuffer(buf);
            Assert.Equal(new byte[] { 1, 2, 3 }, buf.ToArray());

            var buf2 = new byte[3];
            // Only one byte remains; asking for three should throw
            Assert.Throws<EndOfStreamException>(() => r.ReadIntoBuffer(buf2.AsSpan()));
        }

        [Fact]
        public void CopyTo_CopiesExact_And_ThrowsOnInsufficientBytes()
        {
            using var r1 = new BigEndianReader(PsdTestUtils.Ms(1, 2, 3, 4, 5));
            using var dest = new MemoryStream();
            r1.CopyTo(dest, 3);
            Assert.Equal(new byte[] { 1, 2, 3 }, dest.ToArray());
            Assert.Equal(4, r1.ReadByte());

            using var r2 = new BigEndianReader(PsdTestUtils.Ms(10, 11));
            using var dest2 = new MemoryStream();
            Assert.Throws<EndOfStreamException>(() => r2.CopyTo(dest2, 3));
        }

        [Fact]
        public void Skip_Long_UsesPoolingPath_And_Advances()
        {
            int count = 2000; // > 512 to hit pooled path
            var payload = Enumerable.Repeat((byte)0xAA, count).ToArray();
            var data = PsdTestUtils.Concat(payload, new byte[] { 0xCC });
            using var r = new BigEndianReader(PsdTestUtils.Ms(data));
            r.Skip((long)count);
            Assert.Equal(0xCC, r.ReadByte());
        }

        [Fact]
        public void Seek_CanSeek_Position_Works()
        {
            var bytes = new byte[] { 0x10, 0x20, 0x30, 0x40 };
            using var ms = new MemoryStream(bytes, writable: false);
            using var r = new BigEndianReader(ms, EncodingProvider.Latin1);
            Assert.True(r.CanSeek);
            Assert.Equal(0, r.Position);
            Assert.Equal(0x10, r.ReadByte());
            Assert.Equal(1, r.Position);
            r.Seek(2);
            Assert.Equal(2, r.Position);
            Assert.Equal(0x30, r.ReadByte());
        }

        [Fact]
        public void Encoding_Property_Returns_ProvidedEncoding()
        {
            var enc = Encoding.UTF8;
            using var r = new BigEndianReader(PsdTestUtils.Ms(0x00), enc);
            Assert.Same(enc, r.Encoding);
        }

        [Fact]
        public void ReadUnicodeString_Large_UsesArrayPool_And_ReturnsCorrectString()
        {
            int units = 2000; // > 512 bytes, triggers ArrayPool path in reader
            var header = PsdTestUtils.BE32((uint)units);
            var codeUnits = Enumerable.Repeat((ushort)0x0041, units).ToArray(); // 'A' repeated
            var payload = PsdTestUtils.Utf16Be(codeUnits);
            var data = PsdTestUtils.Concat(header, payload, new byte[] { 0x00, 0x00 }); // trailing terminator

            using var r2 = new BigEndianReader(PsdTestUtils.Ms(data), EncodingProvider.Latin1);
            var result = r2.ReadUnicodeString();

            Assert.Equal(new string('A', units), result.String);
            Assert.Equal(4 + units * 2 + 2, result.NumBytesRead);
        }

        [Fact]
        public void ReadIntoBuffer_Drains_Internal_Then_Underlying()
        {
            // Internal buffer has 2 bytes; need 4 -> should pull remaining 2 from underlying stream
            using var r3 = new BigEndianReader(PsdTestUtils.Ms(0x03, 0x04, 0x05), EncodingProvider.Latin1);
            r3.Backtrack(new byte[] { 0x01, 0x02 });
            Span<byte> buf = stackalloc byte[4];
            r3.ReadIntoBuffer(buf);
            Assert.Equal(new byte[] { 1, 2, 3, 4 }, buf.ToArray());
            Assert.Equal(0x05, r3.ReadByte());
        }

        [Fact]
        public void Methods_Throw_When_Reader_Disposed()
        {
            var ms = new MemoryStream(new byte[] { 1, 2, 3 }, writable: false);
            var reader = new BigEndianReader(ms, EncodingProvider.Latin1);
            reader.Dispose();

            Assert.Throws<ObjectDisposedException>(() => reader.ReadByte());
            Assert.Throws<ObjectDisposedException>(() => reader.ReadUInt16());
            var span = new byte[1];
            Assert.Throws<ObjectDisposedException>(() => reader.ReadIntoBuffer(span));
        }

        [Fact]
        public void Methods_Throw_When_Underlying_Stream_Disposed()
        {
            var ms = new MemoryStream(new byte[] { 1, 2, 3 }, writable: false);
            using var reader = new BigEndianReader(ms, EncodingProvider.Latin1);
            ms.Dispose();
            Assert.Throws<ObjectDisposedException>(() => reader.ReadByte());
        }
    }
}
