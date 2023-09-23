using MeshBuilding.Geometry;

namespace MeshBuilding.Mesh;

public struct Dirichlet
{
    public int Node { get; }
    public Func<Point, double> Value { get; }

    public Dirichlet(int node, Func<Point, double> value)
    {
        Node = node;
        Value = value;
    }
}

public struct Neumann
{
    public int BorderStart { get; }
    public int BorderEnd { get; }
    public double Theta { get; }

    public Neumann(int borderStart, int borderEnd, double theta)
    {
        BorderStart = borderStart;
        BorderEnd = borderEnd;
        Theta = theta;
    }
}