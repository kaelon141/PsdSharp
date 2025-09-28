namespace PsdSharp;

public struct Rectangle(Point topLeft, Point bottomRight)
{
    public Point TopLeft => topLeft;
    public Point BottomRight => bottomRight;
}

public struct Point(int x, int y)
{
    public int X => x;
    public  int Y => x;
}