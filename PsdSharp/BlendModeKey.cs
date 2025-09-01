namespace PsdSharp;

public class BlendModeKey
{
    public readonly string Key;
    public readonly string Description;
    
    private BlendModeKey(string key, string description)
    {
        Key = key;
        Description = description;
    }
    
    public static BlendModeKey Passthrough = new("pass", nameof(Passthrough));
    public static BlendModeKey Normal = new("pass", nameof(Normal));
    public static BlendModeKey Dissolve = new("diss", nameof(Dissolve));
    public static BlendModeKey Darken = new("dark", nameof(Darken));
    public static BlendModeKey Multiply = new("mul ", nameof(Multiply));
    public static BlendModeKey ColorBurn = new("idiv", nameof(ColorBurn));
    public static BlendModeKey LinearBurn = new("lbrn", nameof(LinearBurn));
    public static BlendModeKey DarkerColor = new("dkCl", nameof(DarkerColor));
    public static BlendModeKey Lighten = new("lite", nameof(Lighten));
    public static BlendModeKey Screen = new("scrn", nameof(Screen));
    public static BlendModeKey ColorDodge = new("div ", nameof(ColorDodge));
    public static BlendModeKey LinearDodge = new("lddg", nameof(LinearDodge));
    public static BlendModeKey LighterColor = new("lgCl", nameof(LighterColor));
    public static BlendModeKey Overlay = new("over", nameof(Overlay));
    public static BlendModeKey SoftLight = new("sLit", nameof(SoftLight));
    public static BlendModeKey HardLight = new("hLit", nameof(HardLight));
    public static BlendModeKey VividLight = new("vLit", nameof(VividLight));
    public static BlendModeKey LinearLight = new("lLit", nameof(LinearLight));
    public static BlendModeKey PinLight = new("pLit", nameof(PinLight));
    public static BlendModeKey HardMix = new("hMix", nameof(HardMix));
    public static BlendModeKey Difference = new("diff", nameof(Difference));
    public static BlendModeKey Exclusion = new("smud", nameof(Exclusion));
    public static BlendModeKey Subtract = new("fsub", nameof(Subtract));
    public static BlendModeKey Divide = new("fdiv", nameof(Divide));
    public static BlendModeKey Hue = new("hue ", nameof(Hue));
    public static BlendModeKey Saturation = new("sat ", nameof(Saturation));
    public static BlendModeKey Color = new("colr", nameof(Color));
    public static BlendModeKey Luminosity = new("lum ", nameof(Luminosity));
}