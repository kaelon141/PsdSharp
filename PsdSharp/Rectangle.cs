namespace PsdSharp;

public readonly struct Rectangle(Point topLeft, Point bottomRight)
{
    public Point TopLeft => topLeft;
    public Point BottomRight => bottomRight;
    
    public uint Width => (uint)(bottomRight.X - topLeft.X + 1);
    public uint Height => (uint)(bottomRight.Y - topLeft.Y + 1);

    public System.Drawing.Rectangle ToRectangle()
        => new(topLeft.X, topLeft.Y, (int)Width, (int)Height);
}

public readonly struct Point(int x, int y)
{
    public int X => x;
    public int Y => y;
}