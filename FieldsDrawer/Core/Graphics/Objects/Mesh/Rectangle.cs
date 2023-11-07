using System.Linq;
using FieldsDrawer.Core.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BicubicHermite.Core.Graphics.Objects.Mesh;

public class Rectangle : IBaseObject
{
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }
    
    public Point LeftBottom { get; }
    public Point RightTop { get; }

    public Rectangle(Point leftBottom, Point rightTop)
    {
        ObjectType = PrimitiveType.Quads;
        LeftBottom = leftBottom;
        RightTop = rightTop;
        Points = new[]
        {
            leftBottom,
            new(rightTop.X, leftBottom.Y),
            rightTop,
            new(leftBottom.X, rightTop.Y)
        };
        Indices = new uint[] { 0, 1, 2, 3 };
        Colors = new[] { Color4.Black };
        PointSize = 1;
    }
    
    public void BoundingBox(out Point leftBottom, out Point rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;

        leftBottom = new Point(minX, minY);
        rightTop = new Point(maxX, maxY);
    }
}