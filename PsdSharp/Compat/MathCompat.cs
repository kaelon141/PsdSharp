namespace PsdSharp.Compat;

public static class MathCompat
{
    public static float Clamp(float value, float min, float max)
    {
        return value switch
        {
            _ when value < min => min,
            _ when value > max => max,
            _ => value
        };
    }
}