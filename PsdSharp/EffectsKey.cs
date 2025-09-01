namespace PsdSharp;

public class EffectsKey
{
    public readonly string Key;
    public readonly string Description;
    
    private EffectsKey(string key, string description)
    {
        Key = key;
        Description = description;
    }
    
    public static EffectsKey CommonState = new("cmnS", nameof(CommonState));
    public static EffectsKey DropShadow = new("dsdw", nameof(DropShadow));
    public static EffectsKey InnerShadow = new("isdw", nameof(InnerShadow));
    public static EffectsKey OuterGlow = new("oglw", nameof(OuterGlow));
    public static EffectsKey InnerGlow = new("iglw", nameof(InnerGlow));
    public static EffectsKey Bevel = new("bevl", nameof(Bevel));
    public static EffectsKey SolidFill = new("sofi", nameof(SolidFill));
}