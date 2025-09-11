using System.Buffers.Binary;
using System.Text;

namespace PsdSharp.Parsing;

internal class BigEndianReader(Stream input, Encoding encoding, bool leaveOpen = false) : IDisposable
{
    private bool _disposed;
    
    public BigEndianReader(Stream input) : this(input, Encoding.Latin1) {}
    
    public Encoding Encoding => encoding;

    private ReadOnlySpan<byte> InternalRead(Span<byte> buffer)
    {
        ThrowIfDisposed();
        
        input.ReadExactly(buffer);

        return buffer;
    }
    
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("The BigEndianReader has been disposed.");
        }
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && !leaveOpen)
            {
                input.Dispose();
            }
            _disposed = true;
        }
        
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    public void Close()
    {
        Dispose(true);
    }

    public virtual byte ReadByte()
    {
        ThrowIfDisposed();

        int b = input.ReadByte();
        if (b < 0) throw new EndOfStreamException();
        return (byte) b;
    }
    public virtual sbyte ReadSByte()
    {
        ThrowIfDisposed();
        
        int b = input.ReadByte();
        if (b < 0) throw new EndOfStreamException();
        return unchecked((sbyte) b);
    }

    public virtual short ReadInt16()
        => BinaryPrimitives.ReadInt16BigEndian(InternalRead(stackalloc byte[sizeof(short)]));
    public virtual ushort ReadUInt16()
        => BinaryPrimitives.ReadUInt16BigEndian(InternalRead(stackalloc byte[sizeof(ushort)]));
    public virtual int ReadInt32()
        => BinaryPrimitives.ReadInt32BigEndian(InternalRead(stackalloc byte[sizeof(int)]));
    public virtual uint ReadUInt32()
        => BinaryPrimitives.ReadUInt32BigEndian(InternalRead(stackalloc byte[sizeof(uint)]));
    public virtual long ReadInt64()
        => BinaryPrimitives.ReadInt64BigEndian(InternalRead(stackalloc byte[sizeof(long)]));
    public virtual ulong ReadUInt64()
        => BinaryPrimitives.ReadUInt64BigEndian(InternalRead(stackalloc byte[sizeof(ulong)]));
    public virtual Half ReadHalf()
        => BinaryPrimitives.ReadHalfBigEndian(InternalRead(stackalloc byte[sizeof(ushort)]));
    public virtual float ReadSingle()
        => BinaryPrimitives.ReadSingleBigEndian(InternalRead(stackalloc byte[sizeof(float)]));
    public virtual double ReadDouble()
        => BinaryPrimitives.ReadDoubleBigEndian(InternalRead(stackalloc byte[sizeof(double)]));

    /// <summary>
    /// Read a Pascal string. Returns <c>null</c> if the length byte indicates 0 length
    /// (this is the same as a null string)
    /// </summary>
    /// <param name="alignmentSize"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public virtual (string? String, ushort NumBytesRead) ReadPascalString(byte alignmentSize = 2)
    {
        if (alignmentSize is not 2 and not 4)
        {
            throw new ArgumentOutOfRangeException(nameof(alignmentSize));
        }
        
        byte stringLength = ReadByte();
        
        //if string is not present, it is a null string, padded to the alignment size
        //so in that case, skip the rest of the padding so the stream cursor advances to the proper position.
        if (stringLength == 0b0)
        {
            Span<byte> buf = stackalloc byte[alignmentSize - 1];
            InternalRead(buf);
            return (null, alignmentSize);
        }
        
        //put small strings on the stack for performance, otherwise rent heap space
        Span<byte> buffer = stackalloc byte[stringLength];
        InternalRead(buffer);
        
        //consume padding bytes
        var bytesConsumed = 1 + stringLength;
        var remainingPadding = bytesConsumed % alignmentSize;
        if (remainingPadding != 0)
        {
            Span<byte> buf = stackalloc byte[alignmentSize - remainingPadding];
            InternalRead(buf);
        }
        
        var totalBytesConsumed = checked((ushort)(bytesConsumed + (alignmentSize - remainingPadding)));
        return (encoding.GetString(buffer), totalBytesConsumed);
    }

    public virtual (string? String, long NumBytesRead) ReadUnicodeString()
    {
        const int terminationByteCount = 2;
        //upper limit as a sanity check against corrupt data
        const uint maxCodeUnits = 16 * 1024 * 1024;
        
        var amountOfCodeUnits = ReadUInt32();
        if (amountOfCodeUnits == 0)
        {
            Span<byte> nullTerminationBytesBuffer = stackalloc byte[2];
            InternalRead(nullTerminationBytesBuffer);
            EnsureTerminationBytes(nullTerminationBytesBuffer);
            return (null, 6);
        }
        if (amountOfCodeUnits > maxCodeUnits) throw new InvalidDataException("The data is corrupted.");
        
        var byteCount = checked((int)amountOfCodeUnits * UnicodeEncoding.CharSize) + terminationByteCount;
        
        //put small strings on the stack for performance, otherwise rent shared heap space
        if (byteCount <= 512)
        {
            Span<byte> buffer = stackalloc byte[byteCount];
            InternalRead(buffer);
            EnsureTerminationBytes(buffer);
            return (Encoding.BigEndianUnicode.GetString(buffer[..^terminationByteCount]), 4 + buffer.Length);
        }

        byte[] rentedBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            var span = rentedBuffer.AsSpan(0, byteCount);
            InternalRead(span);
            EnsureTerminationBytes(span);
            return (Encoding.BigEndianUnicode.GetString(span[..^terminationByteCount]), 4 + span.Length);
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }

    private void EnsureTerminationBytes(Span<byte> buffer)
    {
        if (buffer[^1] != 0x0 || buffer[^2] != 0x0) throw ParserUtils.DataCorrupt();
    }

    public virtual string ReadSignature()
    {
        Span<byte> buffer = stackalloc byte[4];
        InternalRead(buffer);
        return Encoding.ASCII.GetString(buffer);
    }

    public virtual void ReadIntoBuffer(Span<byte> buffer)
    {
        if (buffer.Length == 0) return;
        
        InternalRead(buffer);
    }

    public void Skip(int numberOfBytes)
    {
        ThrowIfDisposed();
        if(numberOfBytes <= 0) return;

        //for small sizes, use a stack buffer. Otherwise, rent shared heap space.
        if (numberOfBytes < 512)
        {
            Span<byte> buf = stackalloc byte[numberOfBytes];
            InternalRead(buf);
            return;
        }
        
        var skipBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(8192);
        try
        {
            while (numberOfBytes > 0)
            {
                var span = skipBuffer.AsSpan(0, Math.Min(numberOfBytes, skipBuffer.Length));
                InternalRead(span);
                numberOfBytes -= span.Length;
            }
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(skipBuffer);
        }

    }
}