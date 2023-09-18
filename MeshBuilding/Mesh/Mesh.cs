using MeshBuilding.Geometry;

namespace MeshBuilding.Mesh;

public class Mesh
{
    public Point[] Points { get; }
    public FiniteElement[] Elements { get; }

    public Mesh(Point[] points, FiniteElement[] elements)
    {
        Points = points;
        Elements = elements;
    }
}