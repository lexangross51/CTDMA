using System.Globalization;
using MeshBuilding.Geometry;

namespace MeshBuilding;

public static class Utilities
{
    public static void SaveMesh(Mesh.Mesh mesh, string folder)
    {
        var sw = new StreamWriter($"{folder}/points");
        foreach (var p in mesh.Points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.CurrentCulture);
        }
        sw.Close();

        sw = new StreamWriter($"{folder}/elements");
        for (var ielem = 0; ielem < mesh.Elements.Length; ielem++)
        {
            if (mesh.IsElementFictitious(ielem)) continue;
            
            var element = mesh.Elements[ielem];
            var nodes = element.Nodes;
            sw.WriteLine($"{nodes[0]} {nodes[1]} {nodes[2]} {nodes[3]}");
        }

        sw.Close();
    }
    
    public static void WriteNodes(IEnumerable<Point> pointsCollection, string folder, string filename = "nodes")
    {
        var points = pointsCollection.ToArray();
        
        var sw = new StreamWriter($"{folder}/{filename}");
        foreach (var p in points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.CurrentCulture);
        }
        sw.Close();
    }
    
    public static double Distance2D(double ax, double ay, double bx, double by)
    {
        return Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
    }
    
    public static bool IsPointOnSegment(Point a, Point b, Point p)
    {
        var d3 = Distance2D(a.X, a.Y, b.X, b.Y);
        var d1 = Distance2D(p.X, p.Y, a.X, a.Y) / d3;
        var d2 = Distance2D(p.X, p.Y, b.X, b.Y) / d3;

        return Math.Abs(d1 + d2 - 1.0) < 1E-14;
    }
    
    public static bool IsPointOnPolygon(IEnumerable<Point> pointsCollection, Point point)
    {
        var points = pointsCollection.ToArray();
        var pointsCount = points.Length;

        for (int i = 0; i < pointsCount; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % pointsCount];

            if (IsPointOnSegment(p1, p2, point))
                return true;
        }

        return false;
    }

    public static bool IsPointInsidePolygon(Point point, params Point[] points)
    {
        if (IsPointOnPolygon(points, point)) return false;

        int intersections = 0;

        for (int i = 0; i < points.Length; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % points.Length];

            if (!(point.Y > Math.Min(p1.Y, p2.Y)) ||
                !(point.Y <= Math.Max(p1.Y, p2.Y)) ||
                !(point.X <= Math.Max(p1.X, p2.X)) ||
                !(Math.Abs(p1.Y - p2.Y) > 1E-14)) continue;

            var xIntersection = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;

            if (Math.Abs(p1.X - p2.X) < 1E-14 || point.X <= xIntersection)
            {
                intersections++;
            }
        }

        return intersections % 2 == 1;
    }

    public static void MassCenter(IEnumerable<Point> points, out double x, out double y)
    {
        x = 0.0;
        y = 0.0;

        var ps = points.ToArray();

        foreach (var p in ps)
        {
            x += p.X;
            y += p.Y;
        }

        x /= ps.Length;
        y /= ps.Length;
    }
}