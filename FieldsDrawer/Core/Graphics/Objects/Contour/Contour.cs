using System.Collections.Generic;
using System.Linq;
using FieldsDrawer.Core.Graphics.Objects.Mesh;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TriangleNet;
using TriangleNet.Meshing;

namespace FieldsDrawer.Core.Graphics.Objects.Contour;

public class Contour : IBaseObject
{
    private readonly TriangleNet.Meshing.Algorithm.SweepLine _triangulation = new();
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }
    public Mesh.Mesh Mesh { get; } 

    public Contour(IEnumerable<Point> pointsCollection, IEnumerable<double> values, int levels = 5)
    {
        ObjectType = PrimitiveType.Lines;
        PointSize = 1;
        
        var mesh = _triangulation.Triangulate(MathHelper.ToTriangleNetVertices(pointsCollection), new Configuration());
        Mesh = ToSharpPlotMesh(mesh);
        var isolineBuilder = new IsolineBuilder(Mesh, values.ToArray());
        isolineBuilder.BuildIsolines(levels);

        Points = isolineBuilder.Points.ToArray();
        Colors = new[] { Color4.Black };
        Indices = null;
    }
    
    public Contour(IMesh mesh, IEnumerable<double> values, int levels = 5)
    {
        ObjectType = PrimitiveType.Lines;
        PointSize = 1;
        
        Mesh = ToSharpPlotMesh(mesh);
        var isolineBuilder = new IsolineBuilder(Mesh, values.ToArray());
        isolineBuilder.BuildIsolines(levels);

        Points = isolineBuilder.Points.ToArray();
        Colors = new[] { Color4.Black };
        Indices = null;
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

    private Mesh.Mesh ToSharpPlotMesh(IMesh mesh)
    {
        var points = new Point[mesh.Vertices.Count];
        var triangles = new Element[mesh.Triangles.Count];
        int index = 0;

        foreach (var point in mesh.Vertices)
        {
            points[index++] = new Point(point.X, point.Y);
        }

        index = 0;

        foreach (var triangle in mesh.Triangles)
        {
            triangles[index++] = new Element(new[]
            {
                triangle.GetVertexID(0),
                triangle.GetVertexID(1),
                triangle.GetVertexID(2),
            });
        }

        return new Mesh.Mesh(points, triangles);
    }
}
