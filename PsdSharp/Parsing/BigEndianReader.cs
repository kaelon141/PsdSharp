using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace PsdSharp.Parsing;

internal class BigEndianReader(Stream input, Encoding encoding, bool leaveOpen = false) : IDisposable
{
    private bool _disposed;

    private byte[] _internalBuffer = [];
    
    public BigEndianReader(Stream input) : this(input, Encoding.GetEncoding("ISO-8859-1")) {}
    
    public Encoding Encoding => encoding;

    private long _numBytesRead;

    public long Position => input.CanSeek ? input.Position : _numBytesRead;

    private ReadOnlySpan<byte> InternalRead(Span<byte> buffer)
    {
        ThrowIfDisposed();

        if (buffer.Length == 0)
        {
            return buffer;
        }

        // First satisfy from any previously backtracked bytes.
        var numBytesFromInternal = 0;
        if (_internalBuffer.Length > 0)
        {
            numBytesFromInternal = Math.Min(_internalBuffer.Length, buffer.Length);
            _internalBuffer.AsSpan(0, numBytesFromInternal).CopyTo(buffer);

            if (numBytesFromInternal == _internalBuffer.Length)
            {
                // Consumed entire internal buffer.
                _internalBuffer = [];
            }
            else
            {
                // Slice off consumed bytes (keep remaining for future reads).
                var remaining = new byte[_internalBuffer.Length - numBytesFromInternal];
                Array.Copy(_internalBuffer, numBytesFromInternal, remaining, 0, remaining.Length);
                _internalBuffer = remaining;
            }
            
            _numBytesRead += numBytesFromInternal;
        }

        // Read any remaining bytes directly from the underlying stream.
        if (numBytesFromInternal < buffer.Length)
        {
            #if NET6_0_OR_GREATER
                var span = buffer[numBytesFromInternal..];
                var bytesRead = input.Read(span);

                if (bytesRead < span.Length)
                {
                    throw new EndOfStreamException();
                }

                _numBytesRead += bytesRead;
            #else
                var numBytesRemaining = buffer.Length - numBytesFromInternal;
                var rentedBuffer = ArrayPool<byte>.Shared.Rent(numBytesRemaining);
                try
                {
                    var bytesRead = input.Read(rentedBuffer,0, numBytesRemaining);
                    
                    if (bytesRead < numBytesRemaining)
                    {
                        throw new EndOfStreamException();
                    }
                    
                    rentedBuffer.AsSpan(0, bytesRead).CopyTo(buffer.Slice(numBytesFromInternal));
                    _numBytesRead += bytesRead;
                } finally
                {
                    ArrayPool<byte>.Shared.Return(rentedBuffer);
                }
            #endif
        }

        return buffer;
    }
    
    private bool TryReadFromInternalBuffer(out byte value)
    {
        if (_internalBuffer.Length > 0)
        {
            value = _internalBuffer[0];
            if (_internalBuffer.Length == 1)
            {
                _internalBuffer = [];
            }
            else
            {
                var remaining = new byte[_internalBuffer.Length - 1];
                Array.Copy(_internalBuffer, 1, remaining, 0, remaining.Length);
                _internalBuffer = remaining;
            }
            return true;
        }
        value = 0;
        return false;
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

        if (TryReadFromInternalBuffer(out var b1)) return b1;
        int b = input.ReadByte();
        if (b < 0) throw new EndOfStreamException();
        return (byte) b;
    }
    public virtual sbyte ReadSByte()
    {
        ThrowIfDisposed();

        if (TryReadFromInternalBuffer(out var b1)) return unchecked((sbyte)b1);
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

    public virtual float ReadSingle()
    #if NET6_0_OR_GREATER
        => BinaryPrimitives.ReadSingleBigEndian(InternalRead(stackalloc byte[sizeof(float)]));
    #else
        => Compat.BinaryPrimitivesCompat.ReadSingleBigEndian(InternalRead(stackalloc byte[sizeof(float)]));
    #endif
    
    public virtual double ReadDouble()
    #if NET6_0_OR_GREATER
        => BinaryPrimitives.ReadDoubleBigEndian(InternalRead(stackalloc byte[sizeof(double)]));
    #else
        => Compat.BinaryPrimitivesCompat.ReadDoubleBigEndian(InternalRead(stackalloc byte[sizeof(double)]));
    #endif

    /// <summary>
    /// Read a Pascal string. Returns <c>null</c> if the length byte indicates 0 length
    /// (this is the same as a null string)
    /// </summary>
    /// <param name="alignmentSize"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public virtual (string? String, ushort NumBytesRead) ReadPascalString(byte alignmentSize)
    {
        if (alignmentSize is not 0 and not 2 and not 4)
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
        
        Span<byte> buffer = stackalloc byte[stringLength];
        InternalRead(buffer);
        
        var bytesConsumed = 1 + stringLength;
        if (alignmentSize > 0)
        {
            //consume padding bytes
            var remainingPadding = bytesConsumed % alignmentSize;
            if (remainingPadding != 0)
            {
                Span<byte> buf = stackalloc byte[alignmentSize - remainingPadding];
                InternalRead(buf);
                bytesConsumed += buf.Length;
            }
        }

#if NET6_0_OR_GREATER
        return (encoding.GetString(buffer), unchecked((ushort)(bytesConsumed)));
        #else
        return (encoding.GetString(buffer.ToArray()), unchecked((ushort)(bytesConsumed)));
        #endif
    }

    public virtual (string? String, long NumBytesRead) ReadUnicodeString()
    {
        //upper limit as a sanity check against corrupt data
        const uint maxCodeUnits = 16 * 1024 * 1024;
        
        var amountOfCodeUnits = ReadUInt32();
        if (amountOfCodeUnits == 0)
        {
            return (null, 4);
        }
        
        if (amountOfCodeUnits > maxCodeUnits) throw new InvalidDataException("The data is corrupted.");
        
        var byteCount = checked((int)amountOfCodeUnits * UnicodeEncoding.CharSize);
        
        //put small strings on the ReadInt32 for performance, otherwise rent shared heap space
        if (byteCount <= 512)
        {
            Span<byte> buffer = stackalloc byte[byteCount];
            InternalRead(buffer);
            EnsureTerminationBytes(buffer);
            
            #if NET6_0_OR_GREATER
                return (Encoding.BigEndianUnicode.GetString(buffer), 4 + buffer.Length);
            #else
                return (Encoding.BigEndianUnicode.GetString(buffer.ToArray()), 4 + buffer.Length);
            #endif
        }

        var rentedBuffer = ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            var span = rentedBuffer.AsSpan(0, byteCount);
            InternalRead(span);
            EnsureTerminationBytes(span);
            
            #if NET6_0_OR_GREATER
                return (Encoding.BigEndianUnicode.GetString(span), 4 + span.Length);
            #else
                return (Encoding.BigEndianUnicode.GetString(span.ToArray()), 4 + span.Length);
            #endif
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }

    public virtual string ReadFixedLengthString(uint length)
        => ReadFixedLengthString(unchecked((int)length));

    public virtual string ReadFixedLengthString(int length)
    {
        Span<byte> buffer = stackalloc byte[length];
        InternalRead(buffer);
        
        #if NET6_0_OR_GREATER
            return encoding.GetString(buffer);
        #else
            return encoding.GetString(buffer.ToArray());
        #endif
    }

    private void EnsureTerminationBytes(Span<byte> buffer)
    {
        #if NET6_0_OR_GREATER
            if (buffer[^1] != 0x0 || buffer[^2] != 0x0) throw ParserUtils.DataCorrupt();
        #else
            var lastTwoBytes = buffer.Slice(buffer.Length - 2);
            if(lastTwoBytes[0] != 0x0 || lastTwoBytes[1] != 0x0) throw ParserUtils.DataCorrupt();
        #endif
    }

    public virtual string ReadSignature()
        => ReadFixedLengthString(4);

    public virtual byte[] ReadUntilEnd()
    {
        if (input.CanSeek)
        {
            var buffer = new byte[input.Length - input.Position];
            var numBytesRead = input.Read(buffer, 0, buffer.Length);
            
            return numBytesRead < buffer.Length ? throw new EndOfStreamException() : buffer;
        }
        
        var ms = new MemoryStream();
        input.CopyTo(ms);
        return ms.ToArray();
    }
    public virtual void ReadIntoBuffer(Span<byte> buffer)
    {
        if (buffer.Length == 0) return;
        
        InternalRead(buffer);
    }

    public virtual void CopyTo(Stream destination, long amountBytes)
    {
        var buffer = new byte[1024 * 1024 * 16];
        while (amountBytes > 0)
        {
            var bytesToCopy = (int) Math.Min(buffer.Length, amountBytes);
            InternalRead(buffer.AsSpan(0, bytesToCopy));
            destination.Write(buffer, 0, bytesToCopy);
            amountBytes -= bytesToCopy;
        }
    }

    public virtual void CopyTo(Stream destination)
    {
        if (_internalBuffer.Length > 0)
        {
            destination.Write(_internalBuffer, 0, _internalBuffer.Length);
            _internalBuffer = [];
        }
        
        input.CopyTo(destination);
    }

    public void Skip(int numberOfBytes)
        => Skip((long)numberOfBytes);
    public void Skip(long numberOfBytes)
    {
        ThrowIfDisposed();
        if(numberOfBytes <= 0) return;

        //for small sizes, use a stack buffer. Otherwise, rent shared heap space.
        if (numberOfBytes < 512)
        {
            Span<byte> buf = stackalloc byte[unchecked((int)numberOfBytes)];
            InternalRead(buf);
            return;
        }
        
        var skipBuffer = ArrayPool<byte>.Shared.Rent(8192);
        try
        {
            while (numberOfBytes > 0)
            {
                var span = skipBuffer.AsSpan(0, (int) Math.Min(numberOfBytes, skipBuffer.Length));
                InternalRead(span);
                numberOfBytes -= span.Length;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(skipBuffer);
        }
    }
    
    //Story bytes into a temporary buffer, as if seeking backwards.
    //The bytes are stored in a separate buffer as the stream may not be seekable.
    public void Backtrack(byte[] bytes)
    {
        var newBuffer = new byte[_internalBuffer.Length + bytes.Length];
        Array.Copy(_internalBuffer, 0, newBuffer, 0, _internalBuffer.Length);
        Array.Copy(bytes, 0, newBuffer, _internalBuffer.Length, bytes.Length);
        _internalBuffer = newBuffer;
        
        _numBytesRead -= bytes.Length;
    }

    public bool CanSeek => input.CanSeek;
    public void Seek(long position)
        => input.Seek(position, SeekOrigin.Begin);
    public long Length => input.Length;
    public long Read => new StreamReader(input).ReadToEnd().Length;
}