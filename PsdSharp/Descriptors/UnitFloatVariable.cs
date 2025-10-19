using PsdSharp.Parsing;

namespace PsdSharp.Descriptors;

public class UnitFloatVariable : Variable
{
    public class UnitType
    {
        public readonly string Key;
        
        public override bool Equals(object? obj)
            => obj is UnitType other && Equals(other);
        public bool Equals(UnitType other)
            => Key == other.Key;

        public override int GetHashCode()
            => Key.GetHashCode();
        
        private UnitType(string key)
        {
            Key = key;
        }
        
        public static readonly UnitType AngleDegrees = new("#Ang");
        public static readonly UnitType DistanceBasePerInch = new("#Rsl");
        public static readonly UnitType DistanceBase72Ppi = new("#Rlt");
        public static readonly UnitType None = new("#Nne");
        public static readonly UnitType Percent = new("#Prc");
        public static readonly UnitType Pixels = new("#Pxl");
        
        internal static readonly UnitType[] All = [AngleDegrees, DistanceBasePerInch, DistanceBase72Ppi, None, Percent, Pixels];
        internal static Dictionary<string, UnitType> ByKey = All.ToDictionary(x => x.Key);
    }
    
    public readonly UnitType Unit;
    public readonly double ValueDouble;
    public override object Value => ValueDouble;
    
    internal UnitFloatVariable(string key, BigEndianReader reader) : base(key)
    {
        var unit = reader.ReadFixedLengthString(4);
        
        Unit = UnitType.ByKey[unit];
        ValueDouble = reader.ReadDouble();
    }

}