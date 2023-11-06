using System.Globalization;

namespace MeshBuilding.Geometry;

public struct Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static bool TryParse(string? data, out Point? point)
    {
        if (data is null)
        {
            point = null;
            return false;
        }

        data = data.Replace(nameof(Point), "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(" ", "");
        
        var words = data.Split(',', ' ');

        if (words is null or { Length: > 2 })
        {
            point = null;
            return false;
        }

        if (!double.TryParse(words[0], CultureInfo.InvariantCulture, out var x))
        {
            point = null;
            return false;
        }
        
        if (!double.TryParse(words[1], CultureInfo.InvariantCulture, out var y))
        {
            point = null;
            return false;
        }

        point = new Point(x, y);
        return true;
    }
}