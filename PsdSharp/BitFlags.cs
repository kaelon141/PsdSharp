namespace PsdSharp;

public class BitFlags
{
    internal byte Flags;

    internal BitFlags(byte flags)
    {
        Flags = flags;
    }
    
    protected bool GetBit(int position) => (0 != (Flags & (1 << position)));
    protected void SetBit(int position, bool value) => Flags = value ? (byte)(Flags | (1 << position)) : (byte)(Flags & ~(1 << position));
}