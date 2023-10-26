namespace MeshBuilding.Geometry;

public struct Primitives
{
    public double LeftBorder { get; set; }
    public double RightBorder { get; set; }

    public Primitives(double left, double right)
        => (LeftBorder, RightBorder) = (left, right);
    
    public double Length => RightBorder - LeftBorder;
}

public struct Rectangle
{
    public Point LeftBottom { get; set; }
    public Point RightTop { get; set; }

    public Rectangle(Point leftBottom, Point rightTop)
        => (LeftBottom, RightTop) = (leftBottom, rightTop);
    public static double Square(Point leftBottom, Point rightTop) 
        => (rightTop.X - leftBottom.X) * (rightTop.Y - leftBottom.Y);
}