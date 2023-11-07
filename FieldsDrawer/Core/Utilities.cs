using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BicubicHermite.Core.Graphics.Objects.Mesh;
using FieldsDrawer.Core.Graphics.Palette;
using OpenTK.Mathematics;
using TriangleNet.Geometry;
using Edge = BicubicHermite.Core.Graphics.Objects.Mesh.Edge;
using Point = FieldsDrawer.Core.Graphics.Objects.Point;

namespace FieldsDrawer.Core;

public enum ColorInterpolationType
{
    Nearest,
    Linear
}

public static class ColorInterpolator
{
    public static Color4 InterpolateColor(double[] range, double value, Palette palette,
        ColorInterpolationType interpolation = ColorInterpolationType.Linear)
    {
        return interpolation switch
        {
            ColorInterpolationType.Nearest => NearestInterpolation(range, value, palette),
            ColorInterpolationType.Linear => LinearInterpolation(range, value, palette),
            _ => throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, null)
        };
    }
    
    private static Color4 NearestInterpolation(double[] range, double value, Palette palette)
    {
        int rangeNumber = 0;

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (value < range[i] || value > range[i + 1]) continue;
            rangeNumber = i;
            break;
        }
        
        return palette[rangeNumber];
    }
    
    private static Color4 LinearInterpolation(double[] range, double value, Palette palette)
    {
        int rangeNumber = 0; 

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (value < range[i] || value > range[i + 1]) continue;
            rangeNumber = i;
            break;
        }

        var start = palette[rangeNumber];
        var end = rangeNumber == palette.ColorsCount - 1 ? palette[rangeNumber] : palette[rangeNumber + 1];

        var t = (value - range[rangeNumber]) / (range[rangeNumber + 1] - range[rangeNumber]);
        var r = start.R + t * (end.R - start.R);
        var g = start.G + t * (end.G - start.G);
        var b = start.B + t * (end.B - start.B);
        return new Color4((float)r, (float)g, (float)b, 1.0f);
    }
}

public static class MathHelper
{
    private static double Epsilon => 1E-14;

    public static double Distance2D(double ax, double ay, double bx, double by)
    {
        return Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
    }

    public static bool IsIntersected(Point a, Point b, Point c, Point d, out Point? intersection)
    {
        intersection = null;
        var denominator = (a.X - b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X - d.X);

        if (Math.Abs(denominator) < Epsilon) return false;

        var intersectX = (a.X * b.Y - a.Y * b.X) * (c.X - d.X) - (c.X * d.Y - c.Y * d.X) * (a.X - b.X);
        var intersectY = (a.X * b.Y - a.Y * b.X) * (c.Y - d.Y) - (c.X * d.Y - c.Y * d.X) * (a.Y - b.Y);
        intersectX /= denominator;
        intersectY /= denominator;

        if (intersectX < Math.Min(a.X, b.X) && intersectX > Math.Max(a.X, b.X) &&
            intersectX < Math.Min(c.X, d.X) && intersectX > Math.Max(c.X, d.X) &&
            intersectY < Math.Min(a.Y, b.Y) && intersectY > Math.Max(a.Y, b.Y) &&
            intersectY < Math.Min(c.Y, d.Y) && intersectY > Math.Max(c.Y, d.Y))
        
            return false;

        intersection = new Point(intersectX, intersectY);
        return true;
    }
    
    public static bool IsPointOnSegment(Point a, Point b, Point p)
    {
        var d3 = Distance2D(a.X, a.Y, b.X, b.Y);
        var d1 = Distance2D(p.X, p.Y, a.X, a.Y) / d3;
        var d2 = Distance2D(p.X, p.Y, b.X, b.Y) / d3;

        return Math.Abs(d1 + d2 - 1.0) < Epsilon;
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
                !(Math.Abs(p1.Y - p2.Y) > Epsilon)) continue;

            var xIntersection = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;

            if (Math.Abs(p1.X - p2.X) < Epsilon || point.X <= xIntersection)
            {
                intersections++;
            }
        }

        return intersections % 2 == 1;
    }

    public static bool CanDropPerpendicular(Point point, Point a, Point b, out Point pointProjection)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var lenghtSqr = dx * dx + dy * dy;
        var wx = point.X - a.X;
        var wy = point.Y - a.Y;
        var scalar = wx * dx + wy * dy;

        if (scalar > 0 && scalar < lenghtSqr)
        {
            var projX = scalar / lenghtSqr * dx;
            var projY = scalar / lenghtSqr * dy;

            pointProjection = new Point(a.X + projX, a.Y + projY);

            return true;
        }

        pointProjection = new Point();
        return false;
    }

    public static Point InterpolateByValue(Point p1, Point p2, double v1, double v2, double v)
    {
        var t = (v - v1) / (v2 - v1);
        var x = p1.X + t * (p2.X - p1.X);
        var y = p1.Y + t * (p2.Y - p1.Y);
        return new Point(x, y);
    }
    
    public static IList<Vertex> ToTriangleNetVertices(IEnumerable<Point> pointsCollection)
    {
        var pointsArray = pointsCollection.ToArray();
        var points = new Vertex[pointsArray.Length];
        int index = 0;

        foreach (var point in pointsArray)
        {
            points[index++] = new Vertex(point.X, point.Y);
        }

        return points;
    }
}

public class IsolineBuilder
{
    private readonly Mesh _mesh;
    private readonly double[] _values;
    private readonly int[] _binaryMap;
    private List<Edge> _edges;
    public readonly List<Point> Points;

    public IsolineBuilder(Mesh mesh, double[] values)
    {
        _mesh = mesh;
        _values = values;
        _binaryMap = new int[_values.Length];
        Points = new List<Point>();
        _edges = new List<Edge>(3);
    }
    
    private void MakeBinaryMap(double threshold)
    {
        Array.Clear(_binaryMap);

        for (int i = 0; i < _values.Length; i++)
        {
            _binaryMap[i] = _values[i] < threshold ? 0 : 1;
        }
    }
    
    private int GetElementState(Element element)
    {
        _edges.Clear();
        var bin = element.Nodes.Select(node => _binaryMap[node]).ToArray();
        var state = bin[2] * 4 + bin[1] * 2 + bin[0] * 1;
        
        if (state is 0 or 7) return state;
        
        for (int i = 0; i < 3; i++)
        {
            if (bin[i] == 0) continue;
            
            for (int j = 0; j < 3; j++)
            {
                if (element.Edges[j].Contain(element.Nodes[i]))
                {
                    _edges.Add(element.Edges[j]);
                }
            }
        }

        _edges = _edges.GroupBy(edge => edge)
            .Where(item => item.Count() == 1)
            .SelectMany(g => g)
            .ToList();
        
        return state;
    }
    
    public void BuildIsolines(int levels)
    {
        double min = _values.Min();
        double max = _values.Max();
        double step = (max - min) / levels;

        for (int i = 0; i < levels + 1; i++)
        {
            double threshold = min + i * step;
            
            MakeBinaryMap(threshold);

            foreach (var element in _mesh.Elements)
            {
                var state = GetElementState(element);
                
                if (state is 0 or 7) continue;

                var edge = _edges.First();
                var p1 = _mesh.Points[edge.Node1];
                var p2 = _mesh.Points[edge.Node2];
                var v1 = _values[edge.Node1];
                var v2 = _values[edge.Node2];
                Points.Add(MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold));
                
                edge = _edges.Last();                                                  
                p1 = _mesh.Points[edge.Node1];                                          
                p2 = _mesh.Points[edge.Node2];                                          
                v1 = _values[edge.Node1];                                               
                v2 = _values[edge.Node2];                                               
                Points.Add(MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold));
            }
        }
    }
}

public static class FilesWorking
{
    public static (IEnumerable<Point> Points, IEnumerable<double> Values) LoadData(string filename)
    {
        var points = new List<Point>();
        var values = new List<double>();
        
        var separators = new[] { ',', '\t', '\n', ' ' };
        var lines = File.ReadAllLines(filename);

        foreach (var line in lines)
        {
            var lLine = line.Replace(',', '.');
            var words = lLine.Split(separators).Select(val => double.Parse(val, CultureInfo.InvariantCulture)).ToArray();
            points.Add(new Point
            {
                X = words[0],
                Y = words[1],
            });
            values.Add(words[2]);
        }

        var fp = points.First();
        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i];
            p.X -= fp.X;
            p.Y -= fp.Y;
            points[i] = p;
        }

        return (points, values);
    }

    public static void WriteData(string filename, IEnumerable<Point> points, IEnumerable<double> values)
    {
        var p = points.ToArray();
        var v = values.ToArray();
        
        var sw = new StreamWriter(filename);
        for (int i = 0; i < p.Length; i++)
        {
            sw.WriteLine($"{p[i].X} {p[i].Y} {v[i]}");
        }
        
        sw.Close();
    }
}