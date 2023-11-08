using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FieldsDrawer.Core.Graphics.Objects.Mesh;

public class Mesh : IBaseObject
{
    public Element[] Elements { get; }
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }
    public int AbscissaPointsCount { get; }
    public int OrdinatePointsCount { get; }
    
    public Mesh(IEnumerable<Point> points, IEnumerable<Element> elements)
    {
        Points = points.ToArray();
        Elements = elements.ToArray();
        Colors = new[] { Color4.Black };
        Indices = new uint[Elements.Length * Elements[0].Nodes.Length];
        ObjectType = Elements.First().Nodes.Length == 3 ? PrimitiveType.Triangles : PrimitiveType.Quads;
        PointSize = 1;

        uint index = 0;
        if (Elements.First().Nodes.Length == 4)
        {
            foreach (var element in Elements)
            {
                Indices[index++] = (uint)element.Nodes[0];
                Indices[index++] = (uint)element.Nodes[1];
                Indices[index++] = (uint)element.Nodes[3];
                Indices[index++] = (uint)element.Nodes[2];
            }   
        }
        else
        {
            foreach (var node in Elements.SelectMany(element => element.Nodes))
            {
                Indices[index++] = (uint)node;
            }   
        }
        
        AbscissaPointsCount = Points.DistinctBy(p => p.X).Count();
        OrdinatePointsCount = Points.DistinctBy(p => p.Y).Count();
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

    public int FindElementByPoint(double x, double y)
    {
        for (var ielem = 0; ielem < Elements.Length; ielem++)
        {
            var element = Elements[ielem];
            var p1 = Points[element.Nodes.First()];
            var p2 = Points[element.Nodes.Last()];

            if (p1.X <= x && x <= p2.X && p1.Y <= y && y <= p2.Y)
            {
                return ielem;
            }
        }

        return -1;
    }
}
